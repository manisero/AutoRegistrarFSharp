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
let c_r1Reg = { defaultRegistration with classType = typeof<C_R1>; interfaceTypes = [typeof<I_R1>] }
let c_r1_r2Reg = { defaultRegistration with classType = typeof<C_R1_R2>; interfaceTypes = [typeof<I_R1_R2>] }
let c_r1_r1Reg = { defaultRegistration with classType = typeof<C_R1_R1>; interfaceTypes = [typeof<I_R1_R1>] }

// helpers

let assertInvalidOp action =
    (fun () -> action() |> ignore) |> should throw typeof<InvalidOperationException>

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
    (fun () -> getDepTypes typeof<MultiCtors>) |> assertInvalidOp

// findReg

let findRegCases =
    [
        (typeof<R1>, [r1Reg], r1Reg);
        (typeof<R2>, [r1Reg; r2Reg], r2Reg);
        (typeof<R2_Base>, [r1Reg; r2Reg], r2Reg);
        (typeof<I2_1>, [r1Reg; r2Reg], r2Reg)
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
let ``findReg: -> matching reg`` case =
    let (typ, regs, exp) = findRegCases.[case]

    let res = findReg typ regs

    res |> should equal exp

let findRegErrorCases =
    [
        (typeof<R2>, [r1Reg]);
        (typeof<R2_Base>, [r1Reg]);
        (typeof<I2_1>, [r1Reg])
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
let ``findReg: no matching reg -> error`` case =
    let (typ, regs) = findRegErrorCases.[case]

    (fun () -> findReg typ regs) |> assertInvalidOp

// BuildDependencyGraph

let getRegCopies() =
    [
        { r1Reg with classType = r1Reg.classType };
        { r2Reg with classType = r2Reg.classType };
        { c_r1Reg with classType = c_r1Reg.classType };
        { c_r1_r2Reg with classType = c_r1_r2Reg.classType };
        { c_r1_r1Reg with classType = c_r1_r1Reg.classType }
    ]

let assertRegDeps expDeps reg =
    reg.dependencies |> should equal expDeps

let dependencyGraphCases =
    [
        (r1Reg.classType, []);
        (r2Reg.classType, []);
        (c_r1Reg.classType, [r1Reg]);
        (c_r1_r2Reg.classType, [r1Reg; r2Reg]);
        (c_r1_r1Reg.classType, [r1Reg])
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
[<InlineData(4)>]
let ``BuildDependencyGraph: -> deps filled`` case =
    let (regClass, expDeps) = dependencyGraphCases.[case]
    let regs = getRegCopies()

    BuildDependencyGraph regs

    regs |> List.find (fun x -> x.classType = regClass) |> assertRegDeps expDeps
