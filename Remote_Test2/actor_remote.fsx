#time "on"
#r "nuget: Akka"
#r "nuget: Akka.FSharp"
#r "nuget: Akka.TestKit"
#r "nuget: Akka.Remote"
#load "sha256.fsx"
#load "RandomStringRemote.fsx"

open System.Threading
open Akka.Routing
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
                    transport-protocol = tcp
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
    | Mine
    | Found
type RemoteBossMessage = 
    | MineJobBoss of numZero : int * strLen : int

// let scriptArg = fsi.CommandLineArgs
let mutable numLead = 0

type childMiner() = 
    inherit Actor()
    override x.OnReceive message = 
        let mainBoss = system.ActorSelection( "akka.tcp://system@localhost:8777/user/boss")
        
        let x : MinerMessage = downcast message
        match x with 
        | MineJob(numZero, strLen) ->
            // let mainBoss = system.ActorSelection("akka.tcp://system@localhost:8777/user/boss")
            
            let inputStr = RandomStringRemote.inputStr strLen
            //printf "%s  " inputStr
            let hash = Sha256.stringToHash inputStr
            //printf "%s\n" hash
            let found = RandomStringRemote.leadCheck (hash, numZero)
            if(found) then 
                printf "[[ Found Miner: ]] %s : %s\n" inputStr hash
                mainBoss <! Found
        | Stop ->
            mainBoss <! Found
        | _ -> ()
    

let remoteBoss = 
    spawn system "boss"
    <| fun mailbox ->
        let numProcess = System.Environment.ProcessorCount |> int
        let numMiners = numProcess*125
        //let minerArray = Array.create numMiners (spawn mailbox.Context  "miner" Miner)
        let childMinerSet =
            [1 .. numMiners]
            |> List.map(fun id -> system.ActorOf(Props(typedefof<childMiner>)))

        let childCount = [|for cm in childMinerSet -> cm|]
        let minerSystem = system.ActorOf(Props.Empty.WithRouter(RoundRobinGroup(childCount)))
        
        let rand = new Random()
        let rec loop() = actor {
            
            let! (msg : obj) = mailbox.Receive()
            let (numZero, strLen) : Tuple<int32 ,int32> = downcast msg
            
            minerSystem <! MineJob(numLead,strLen)

            // match msg with
            //     | MineJobBoss(numZero, strLen) ->
            //         numLead <- numZero
            //         let minerArray = Array.create numMiners (spawn mailbox.Context ("miner" + RandomStringRemote.randomStr (rand.Next(1000))) childMiner)
            //         for i in minerArray do
            //             i <! MineJob(numLead,rand.Next(100))
            //         ()
            //     | Mine -> 
            //         let minerArray = Array.create numMiners (spawn mailbox.Context ("miner" + RandomStringRemote.randomStr (rand.Next(1000))) childMiner)
            //         for i in minerArray do
            //             i <! MineJob(numLead,rand.Next(100))
            //         ()
            //     | Found -> 
            //     mailbox.Context.System.Terminate() |> ignore
            //     ()
            //     | _ -> ()
            return! loop()
        } 
        printfn "Remote Mining Boss has been initialized"
        loop()

system.WhenTerminated.Wait()