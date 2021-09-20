open System

let randomStr (len : int) : string =
    let rand = new Random() 
    let ch = Array.concat [| [|'a' .. 'z'|];[|'A' .. 'Z'|];[|'0' .. '9'|] |]
    let sz = Array.length ch in
    System.String(Array.init len (fun _ -> ch.[rand.Next sz]))

let inputStr (len : int) : string =
    let prefix = "notmarcus"
    prefix + (randomStr len) 

let e = inputStr 5
printf "%s" e
