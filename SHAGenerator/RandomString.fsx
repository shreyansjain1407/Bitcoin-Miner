open System

let str = "adobra;kjsdfk11"

let randomStr (len : int) : string =
    let rand = new Random() 
    let ch = Array.concat [| [|'a' .. 'z'|];[|'A' .. 'Z'|];[|'0' .. '9'|] |]
    let sz = Array.length ch in
    System.String(Array.init len (fun _ -> ch.[rand.Next sz]))

let inputStr (len : int) : string =
    let prefix = "notmarcus"
    prefix + (randomStr len) 

let getLead(str : string, numZero : int) : string =
    let lead = str.[0..numZero]
    lead

let leadCheck(str : string, numZero : int) : bool =
    let zeroArray = [| for i in 1 .. numZero -> 0 |]
    let leadZero = System.String.Join("", zeroArray)
    str.Equals(leadZero)

//let e = inputStr 5
//printf "%s" e
//let x = 1
//printf "%s" (str.[0..x])
//let y = str.[0..x]
//printf "\n%s" y
//let zeroArray = [| for i in 1 .. 3 -> 0 |] 
//let s = System.String.Join("", zeroArray)
