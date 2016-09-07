module DependencyGraphTests

open System
open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open DependencyGraph

// test data

let r1Reg = { defaultRegistration with classType = typeof<R1>; interfaceTypes = [typeof<I1>] }
let r2Reg = { defaultRegistration with classType = typeof<R2>; interfaceTypes = [typeof<R2_Base>; typeof<I2_1>; typeof<I2_2>] }

// helpers

let assertFails exceptionType action =
    (fun () -> action() |> ignore) |> should throw exceptionType

// BuildDependencyGraph

[<Fact>]
let ``no ctor args -> no deps``() =
    let reg = { r1Reg with classType = r1Reg.classType }

    BuildDependencyGraph [reg]

    reg.dependencies |> should equal List.empty<Registration>

// getDependencyTypes

[<Theory>]
[<InlineData(typeof<R1>, null, null)>]
[<InlineData(typeof<C_R1>, typeof<R1>, null)>]
[<InlineData(typeof<C_R1_R2>, typeof<R1>, typeof<R2>)>]
[<InlineData(typeof<C_R1_R1>, typeof<R1>, null)>]
let ``getDepTypes: -> ctor args`` clas (expDep1:Type) (expDep2:Type) =
    let exp = [|expDep1; expDep2|] |> Array.filter (fun x -> x <> null)

    let res = getDepTypes clas

    res |> should equal exp

[<Fact>]
let ``getDepTypes: multiple ctors -> error``() =
    (fun () -> getDepTypes typeof<MultiCtors>) |> assertFails typeof<InvalidOperationException>

// findReg

let findRegCases =
    [
        (typeof<R1>, [r1Reg], r1Reg);
        (typeof<R2>, [r1Reg; r2Reg], r2Reg);
        (typeof<R2_Base>, [r1Reg; r2Reg], r2Reg);
        (typeof<I2_1>, [r1Reg; r2Reg], r2Reg);
        (typeof<I2_2>, [r1Reg; r2Reg], r2Reg)
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
[<InlineData(4)>]
let ``findReg: -> matching reg`` case =
    let (typ, regs, exp) = findRegCases.[case]

    let res = findReg typ regs

    res |> should equal exp
