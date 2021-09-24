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
                    port = 0 
                    hostname = localhost
                }
            }
        }"

let clientSystem = System.create "client" configuration

let serveRef = select clientSystem "akka.tcp://Server@localhost:9002/user/server"
serveRef <! Message(10)
//printfn "Please enter something this is line one"
Console.ReadLine()