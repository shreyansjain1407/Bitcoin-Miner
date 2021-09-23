module Remote_Actor.Main_Actor_fsx
#r "nuget: Akka.FSharp"
#r "nuget: Akka.Remote"

open System
open Akka.FSharp
open Akka.Actor
open Akka.Remote

let deployRemotely address = Deploy(RemoteScope (Address.Parse address))
let spawnRemote systemOrContext remoteSystemAddress actorName expr =
    spawne systemOrContext actorName expr [SpawnOption.Deploy (deployRemotely remoteSystemAddress)]
let system = System.create "local-system" config
let aref =
    spawnRemote system "akka.tcp://remote-system@localhost:9001/" "hello"
       // actorOf wraps custom handling function with message receiver logic
       <@ actorOf (fun msg -> printfn "received '%s'" msg) @>

// send example message to remotely deployed actor
aref <! "Hello world"

// thanks to location transparency, we can select 
// remote actors as if they where existing on local node
let sref = select "akka://local-system/user/hello" system
sref <! "Hello again"

// we can still create actors in local system context
let lref = spawn system "local" (actorOf (fun msg -> printfn "local '%s'" msg))
// this message should be printed in local application console
lref <! "Hello locally"