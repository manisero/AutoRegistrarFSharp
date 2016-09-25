module TestsHelpers

open System
open FsUnit.Xunit

let assertEqualsOption exp item =
    match Option.isSome exp with
    | true ->
        item |> should not' (be Null)
        item |> Option.get |> should equal (Option.get exp)
    | false -> item |> should be Null

let assertContains items list =
    items |> List.iter (fun x -> list |> should contain x)

let assertNotContains items list =
    items |> List.iter (fun x -> list |> should not' (contain x))

let assertInvalidOp action =
    (fun () -> action() |> ignore) |> should throw typeof<InvalidOperationException>
