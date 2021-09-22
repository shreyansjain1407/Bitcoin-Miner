//#time "on"
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

        let e = inputStr strLen
        printf "%s\n" e
        let d = stringToHash e
        printf "%s\n" d
        let found = leadCheck (d, numZero)
        if(found) then 
            printf "found: %s : %s\n" e d
            boss <! BossJob(found, e, d)       
        return! loop()
    }
    loop()

let processorRef = spawn system "miner" Miner

processorRef <! MineJob(1,5)

let Boss (mailbox : Actor <_>) = 
    let numProcess = System.Environment.ProcessorCount |> int64
    let numMiners = numProcess*250L
    let miners = 
        [1L..numMiners]
        |> List.map(fun id -> spawn system (sprintf "local_%d" id) Miner)
    let minerEnum = [| for m in miners -> m |]
    
    let mutable finished = 0L
    let split = numMiners * 2L

    let rec loop() = actor {
        let! BossJob(found, input, hash) = mailbox.Receive()
        let miner = mailbox.Sender()
        miner <! MineJob(1,5)
        if (found) then
            printf "found: %s : %s\n" input, hash 
            mailbox.Context.System.Terminate() |> ignore
        return! loop()
    } loop()

let bossRef = spawn system "boss" Boss

system.WhenTerminated.Wait()
