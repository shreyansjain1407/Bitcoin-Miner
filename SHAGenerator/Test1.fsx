open System.IO
open System.Security.Cryptography
open System.Text

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
    printfn "\n %s" (sb'.ToString())
    sb'.ToString() |> str_byt |> (new SHA256Managed()).ComputeHash |> Seq.iter (printf "%x")
    printfn "\n"
    sb'.Length <- sb'.Length - 1;
    
let byteToHex : byte -> string =
    fun b -> b.ToString("x2")

let bytesToHex : byte array -> string =
    fun bytes -> bytes |> Array.fold(fun a x -> a + (byteToHex x)) ""

let stringToHash (_str : string) : string =
    System.Security.Cryptography.SHA256.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes _str) |> bytesToHex
 

let sb_final = System.Text.StringBuilder("shreyansjain")
let find_Hash (hash_Sb : StringBuilder) (level : int) =
    if level = 5 then
        let cur_hash = stringToHash (hash_Sb.ToString())
        
        0
    else
        for i = 32 to 126 do
            hash_Sb.Append(intToChar i)
            find_Hash hash_Sb (level + 1)
            hash_Sb.Length <- sb'.Length - 1;
        
        0