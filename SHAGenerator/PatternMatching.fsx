open System

let check = "00dsfiuhasdija0asdfiugsvv5asdfjbn"
let (|Prefix|_|) (p:string) (s:string) =
    if s.StartsWith(p) then
        Some(s.Substring(p.Length))
    else
        None

match check with
| Prefix "00" rest -> printfn "Started with '00', rest is %s" rest
//| Prefix "Hello" rest -> printfn "Started with 'Hello', rest is %s" rest
| _ -> printfn "neither"