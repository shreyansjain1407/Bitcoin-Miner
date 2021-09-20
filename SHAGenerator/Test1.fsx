open System.IO
open System.Security.Cryptography

for arg in fsi.CommandLineArgs |> Seq.skip 1 do
    printf "Calculating sha256 of %s\n  " arg
    File.ReadAllBytes(arg) |> (new SHA256Managed()).ComputeHash |> Seq.iter (printf "%x")
    printfn ""

let str_byt (str:string) = 
    let out = System.Text.Encoding.ASCII.GetBytes(str)
    out

let sb = System.Text.StringBuilder("ShreyansJain")

// printfn File.ReadAllBytes("inp.txt")
// let hash = File.ReadAllBytes("inp.txt") |> (new SHA256Managed()).ComputeHash
// let hash = System.Text.Encoding.ASCII.GetBytes(sb.ToString()) |> (new SHA256Managed()).ComputeHash |> Seq.iter (printf "%x")
let hash' = sb.ToString() |> str_byt |> (new SHA256Managed()).ComputeHash |> Seq.iter (printf "%x")

let intToChar c = 
    char c

printfn "%c" (intToChar 48)
let sb' = System.Text.StringBuilder("ShreyansJain")
for i = 32 to 126 do
    sb'.Append(intToChar i)
    printfn "\n %s \n" (sb'.ToString())
    sb'.ToString() |> str_byt |> (new SHA256Managed()).ComputeHash |> Seq.iter (printf "%x")
    sb'.Length <- sb'.Length - 1;
    