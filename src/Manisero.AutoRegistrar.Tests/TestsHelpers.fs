module TestsHelpers

open System
open FsUnit.Xunit

let assertInvalidOp action =
    (fun () -> action() |> ignore) |> should throw typeof<InvalidOperationException>
