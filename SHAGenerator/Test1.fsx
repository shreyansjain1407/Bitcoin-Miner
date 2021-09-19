open System.IO
open System.Security.Cryptography

for arg in fsi.CommandLineArgs |> Seq.skip 1 do
    printf "Calculating sha256 of %s\n  " arg
    File.ReadAllBytes(arg) |> (new SHA256Managed()).ComputeHash |> Seq.iter (printf "%x")
    printfn ""

let hash = File.ReadAllBytes("inp.txt") |> (new SHA256Managed()).ComputeHash |> Seq.iter (printf "%x")
printfn hash