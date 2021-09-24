#load "Message.fsx"
#r "nuget: Akkling" 
#r "nuget: Akka.Remote"
#r "nuget: Newtonsoft.Json"
// open Message
open System
open Akkling

type Message = Message of int

let configuration = 
    Configuration.parse
        @"akka {
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            }
            remote {
                helios.tcp {
                    port = 9002
                    hostname = localhost
                }
            }
        }"

let serversystem = System.create "Server" configuration

let rec server = function
| Message(num) ->
    printfn "Got a number %d" num
    become server


printfn "Can you see this debug text?"
let serveRef = spawn serversystem "server" <| props(actorOf server)
Console.ReadLine() |> ignore