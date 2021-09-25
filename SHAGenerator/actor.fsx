#time "on"
#r "nuget: Akka.FSharp" //load with nuget
#load "sha256.fsx"
#load "RandomString.fsx"
open System
open Akka.Actor
open Akka.FSharp
open Akka.Configuration
open Sha256
open RandomString
open System



let system = System.create "system" <| Configuration.load()

//type MinerMessage = MineJob of int * int
//type BossMessage = BossJob of bool *  string * string
type MinerMessage = 
    | MineJob of numZero : int * strLen : int
    | Continue
    | Stop
type BossMessage = 
    | Mine
    | Found

let scriptArg = fsi.CommandLineArgs
let numLead = scriptArg.[1] |> int

let Miner (mailbox : Actor<_>)  =
    let rec loop() = actor {
        //let! MineJob(numZero, strLen) = mailbox.Receive()
        let! (msg : MinerMessage) = mailbox.Receive()
        let boss = mailbox.Sender()
        match msg with
            | MineJob(numZero, strLen) ->
                let boss = mailbox.Sender()
                let inputStr = inputStr strLen
                //printf "%s  " inputStr
                let hash = stringToHash inputStr
                //printf "%s\n" hash
                let found = leadCheck (hash, numZero)
                if(found) then 
                    printf "[[ Found Miner: ]] %s : %s\n" inputStr hash
                    boss <! Found
                else
                    boss <! Mine
             | Continue ->
                 boss <! Mine
             | Stop ->
                boss <! Found
             | _ -> ()
            //boss <! found
            //boss <! BossJob(found, inputStr, hash)
            //boss <! BossJob(found, e, d)
            //mailbox.Self <! MineJob(numLead,rand.Next(100))
            //mailbox.Self <! Continue
            //select "user/Boss" mailbox.Context.System <! Mine
        return! loop()
    } loop()
    

let Boss (mailbox : Actor<_>) = 
    let numProcess = System.Environment.ProcessorCount |> int
    let numMiners = 125
    //let minerArray = Array.create numMiners (spawn mailbox.Context  "miner" Miner)
   
    let rand = new Random()
    //for i in minerArray do
               //i <! MineJob(numLead,rand.Next(100))
    let rec loop() = actor {
        //let! BossJob(found, input, hash) = mailbox.Receive()
        //if (found) then
            //printf "[[ Found Boss ]]: %s : %s\n" input hash 
        //for i in minerArray do
            //mailbox.Context.Stop(i)
            //mailbox.Context.System.Terminate() |> ignore
        let! (msg : BossMessage) = mailbox.Receive()
        match msg with
            | Mine -> 
                let minerArray = Array.create numMiners (spawn mailbox.Context ("miner" + randomStr (rand.Next(1000))) Miner)
                for i in minerArray do
                    i <! MineJob(numLead,rand.Next(100))
                ()
            | Found -> 
                mailbox.Context.System.Terminate() |> ignore
                ()
            | _ -> ()

        return! loop()
    } loop()

let bossRef = spawn system "boss" Boss
bossRef <! Mine

system.WhenTerminated.Wait()
