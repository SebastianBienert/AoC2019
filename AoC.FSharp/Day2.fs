module Day2

open System.IO
open System

let input = File.ReadAllText("data/day2.txt").Split ','
            |> Array.map int
            |> Array.toList

let replace (list, index: int, value) = list |> List.mapi(fun i x -> if i = index then value else x)
            
let part1 list = list |> List.iteri(fun i x -> if i % 4 = 0 then replace)