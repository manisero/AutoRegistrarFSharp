module ImplementationMapTests

open System.Linq
open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open ImplementationMap


// getClassInterfaces

let getClassInterfacesCases =
    [
        (typeof<NoInters>, [||]);
        (typeof<R1>, [|typeof<R1>|]);
        (typeof<R2>, [|typeof<R2_Base>; typeof<IR2_1>; typeof<IR2_2>|])
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
let ``getClassInterfaces: class -> immediate base class and implemented interfaces`` case =
    let (typ, expInter) = getClassInterfacesCases.[case]

    let res = getClassInterfaces typ

    res.ToArray() |> should equal expInter
