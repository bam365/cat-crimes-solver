open System
open Game
open Problems


[<EntryPoint>]
let main args = 
    let problem = Int32.Parse(args.[0])
    problemLogic
    |> Map.find problem
    |> solve
    |> List.map configurationToString
    |> String.concat "\n\n"
    |> fun solutions -> Console.WriteLine (sprintf "Solutions to problem %d:\n%s" problem solutions)
    |> fun _ -> 0