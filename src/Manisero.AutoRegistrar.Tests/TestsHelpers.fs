module TestsHelpers

open System
open FsUnit.Xunit

let assertInvalidOp action =
    (fun () -> action() |> ignore) |> should throw typeof<InvalidOperationException>

let assertContains items list =
    items |> List.iter (fun x -> list |> should contain x)

let assertNotContains items list =
    items |> List.iter (fun x -> list |> should not' (contain x))
