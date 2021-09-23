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
open System.Text
open System.Text.RegularExpressions

let system = System.create "system" <| Configuration.load()

//Message Definitions
type MinerMessage = MineJob of int * int
type BossMessage = BossJob of bool * string * string

//Reading Command Line Arguments
let scriptArg = fsi.CommandLineArgs
let numLead = scriptArg.[1] |> int
// printf "Arg: %s\n" scriptArg.[1]

//------Functions to get the hash as a String------//

let byteToHex : byte -> string =
    fun b -> b.ToString("x2")

let bytesToHex : byte array -> string =
    fun bytes -> bytes |> Array.fold(fun a x -> a + (byteToHex x)) ""

//Funtion to be called to get hash
let stringToHash (_str : string) : string =
    System.Security.Cryptography.SHA256.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes _str) |> bytesToHex
//-------------------------------------------------//

let sb = System.Text.StringBuilder("shreyansjain")

let Miner (mailbox : Actor <_>) =
    let rec loop() = actor {
        let! MineJob(numZero, strLen) = mailbox.Receive()
        let boss = mailbox.Sender()
        let e = inputStr strLen
        //printf "%s  " e
        let d = stringToHash e
        //printf "%s\n" d
        let found = leadCheck (d, numZero)
        if(found) then 
            //printf "Found Miner: %s : %s\n" e d
            boss <! BossJob(found, e, d)
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

    let rand = new Random()
    for i in minerArray do
               i <! MineJob(numLead,rand.Next(100))
    let rec loop() = actor {
        let! BossJob(found, input, hash) = mailbox.Receive()
       
        if (found) then
            printf "Found Boss: %s : %s\n" input hash 
            mailbox.Context.System.Terminate() |> ignore
        return! loop()
    } loop()

let bossRef = spawn system "boss" Boss

system.WhenTerminated.Wait()
