open System.IO
open System.Security.Cryptography

for arg in fsi.CommandLineArgs |> Seq.skip 1 do
    printf "Calculating sha256 of %s\n  " arg
    File.ReadAllBytes(arg) |> (new SHA256Managed()).ComputeHash |> Seq.iter (printf "%x")
    printfn ""

let str_byt (str:String) = 
    System.Text.Encoding.ASCII.GetBytes(str)

let sb = System.Text.StringBuilder("ShreyansJain")

printfn File.ReadAllBytes("inp.txt")
// let hash = File.ReadAllBytes("inp.txt") |> (new SHA256Managed()).ComputeHash
let hash = System.Text.Encoding.ASCII.GetBytes(sb.ToString()) |> (new SHA256Managed()).ComputeHash |> Seq.iter (printf "%x")
printfn hash