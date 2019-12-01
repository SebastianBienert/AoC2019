
module Day1 
open System.IO
open System

let input = File.ReadAllLines("data/day1.txt")
            |> Array.filter (not << String.IsNullOrWhiteSpace)
            |> Array.map int

let part1 = input 
             |> Array.map(fun x -> (x / 3) - 2)
             |> Array.sum

let rec CalculateFuel x =
    if ((x / 3) - 2 <= 0) then 0
    else (x / 3) - 2 + CalculateFuel((x / 3) - 2)

let part2 = input
            |> Array.map CalculateFuel
            |> Array.sum



