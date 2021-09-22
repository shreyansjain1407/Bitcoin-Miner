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
(*
let system = System.create "system" <| Configuration.load()

type ProcessorMessage = ProcessJob of int * int

let processor (mailbox : Actor <_>) =
    let rec loop() = actor {
        let! ProcessJob(numZero, strLen) = mailbox.Receive()
        // Handle message here
        //printfn "Processor: Received ProcessJob %i %i %i" x y z
        let mutable notFound = true
        while notFound do 
            let e = inputStr strLen
            printf "%s\n" e
            let d = stringToHash e
            printf "%s\n" d
            if(leadCheck (d, numZero)) then 
                printf "found: %s : %s\n" e d
                notFound <- false
        return! loop()
    }
    loop()

let processorRef = spawn system "processor" processor

processorRef <! ProcessJob(1,5)
*)
let mutable notFound = true
while notFound do 
    let e = inputStr 5
    printf "%s\n" e
    let d = stringToHash e
    printf "%s\n" d
    if(leadCheck (d, 2)) then 
        printf "found: %s : %s\n" e d
        notFound <- false
(*
let strategy =
    Strategy.OneForOne (fun e ->
    match e with
    | :? ArithmeticException -> Directive.Resume
    | :? ArgumentException -> Directive.Stop
    | _ -> Directive.Escalate)

let boss =
    spawnOpt system "master" (fun mailbox ->
        let worker = spawn mailbox "worker" workerFun
        
        let rec loop() =
            actor {
                let! msg = mailbox.Receive()
                match msg with
                | Responnd -> worker.Tell(msg, mailbox.Sender())
                | _ -> worker <! msg
                return! loop()
            }
        loop()) [SupervisorStrategy(strategy)]
        *)