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

let system = System.create "system" <| Configuration.load()

type MinerMessage = MineJob of int * int
type BossMessage = BossJob of bool * string * string

let Miner (mailbox : Actor <_>) =
    let rec loop() = actor {
        let! MineJob(numZero, strLen) = mailbox.Receive()
        let boss = mailbox.Sender()
        let numZero = 1
        let strLen = 5
        let e = inputStr strLen
        //printf "%s  " e
        let d = stringToHash e
        //printf "%s\n" d
        let found = leadCheck (d, numZero)
        if(found) then 
            printf "found: %s : %s\n" e d
    
        boss <! BossJob(found, e, d)
        return! loop()
    }
    loop()

//let processorRef = spawn system "miner" Miner

//processorRef <! MineJob(1,5)

let Boss (mailbox : Actor <_>) = 
    let numProcess = System.Environment.ProcessorCount |> int
    let numMiners = numProcess*250
    let minerArray = Array.create numMiners (spawn system "miner" Miner)

    for i in minerArray do
               i <! MineJob(1,5)
    let rec loop() = actor {
        let! BossJob(found, input, hash) = mailbox.Receive()
        let miner = mailbox.Sender()
       
        //workerSystem <! MineJob(1,5)
        if (found) then
            printf "found: %s : %s\n" input, hash 
            mailbox.Context.System.Terminate() |> ignore
        return! loop()
    } loop()

let bossRef = spawn system "boss" Boss

//system.WhenTerminated.Wait()
