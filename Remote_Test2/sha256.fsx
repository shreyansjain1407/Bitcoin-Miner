open System
open System.IO
open System.Security.Cryptography
open System.Text

let str = "adobra;kjsdfk11"

let byteToHex : byte -> string =
    fun b -> b.ToString("x2")

let bytesToHex : byte array -> string =
    fun bytes -> bytes |> Array.fold(fun a x -> a + (byteToHex x)) ""

let stringToHash (_str : string) : string =
    System.Security.Cryptography.SHA256.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes _str) |> bytesToHex

//let d = stringToHash str
