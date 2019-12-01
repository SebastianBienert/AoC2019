module AoC

open System

[<EntryPoint>]
let main argv =
    printfn "1: %d\n" FsharpDay1.part1
    printfn "2: %d" FsharpDay1.part2
    Console.ReadLine() |> ignore
    0