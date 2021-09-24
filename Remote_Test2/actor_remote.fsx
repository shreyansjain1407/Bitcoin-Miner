open System.Threading
#time "on"
#r "nuget: Akka"
#r "nuget: Akka.FSharp"
#r "nuget: Akka.TestKit"
#r "nuget: Akka.Remote"
#load "sha256.fsx"
#load "RandomStringRemote.fsx"


open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit

//configuration
let configuration = 
    ConfigurationFactory.ParseString(
        @"akka {
            log-config-on-start : on
            stdout-loglevel : DEBUG
            loglevel : ERROR
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                debug : {
                    receive : on
                    autoreceive : on
                    lifecycle : on
                    event-stream : on
                    unhandled : on
                }
            }
            remote {
                helios.tcp {
                    port = 8778
                    hostname = localhost
                }
            }
        }")

let system = ActorSystem.Create("RemoteSystem", configuration)

type MinerMessage = 
    | MineJob of numZero : int * strLen : int
    | Continue
    | Stop
type BossMessage = 
    | MineJobBoss of numZero : int * strLen : int
    | Mine
    | Found

// let scriptArg = fsi.CommandLineArgs
let mutable numLead = 0

let Miner (mailbox : Actor<_>)  =
    let rec loop() = actor {
        //let! MineJob(numZero, strLen) = mailbox.Receive()
        let! (msg : MinerMessage) = mailbox.Receive()
        let boss = system.ActorSelection("akka.tcp://system@localhost:8777/user/boss")
        match msg with
            | MineJob(numZero, strLen) ->
                let boss = system.ActorSelection("akka.tcp://system@localhost:8777/user/boss")
                let inputStr = RandomStringRemote.inputStr strLen
                //printf "%s  " inputStr
                let hash = Sha256.stringToHash inputStr
                //printf "%s\n" hash
                let found = RandomStringRemote.leadCheck (hash, numZero)
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
            | MineJobBoss(numZero, strLen) ->
                numLead <- numZero
                let minerArray = Array.create numMiners (spawn mailbox.Context ("miner" + RandomStringRemote.randomStr (rand.Next(1000))) Miner)
                for i in minerArray do
                    i <! MineJob(numLead,rand.Next(100))
                ()
            | Mine -> 
                let minerArray = Array.create numMiners (spawn mailbox.Context ("miner" + RandomStringRemote.randomStr (rand.Next(1000))) Miner)
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