module DependencyGraphTests

open System
open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open DependencyGraph

// test data

let r1Reg = { defaultRegistration with classType = typeof<R1>; interfaceTypes = Some [typeof<IR1>] }
let r2Reg = { defaultRegistration with classType = typeof<R2>; interfaceTypes = Some [typeof<R2_Base>; typeof<IR2_1>; typeof<IR2_2>] }
let c1aReg = { defaultRegistration with classType = typeof<C1A_R1>; interfaceTypes = Some [typeof<IC1A_R1>] }
let c1bReg = { defaultRegistration with classType = typeof<C1B_R1_R2>; interfaceTypes = Some [typeof<IC1B_R1_R2>] }
let c1cReg = { defaultRegistration with classType = typeof<C1C_R1_R1>; interfaceTypes = Some [typeof<IC1C_R1_R1>] }

// getDepTypes

[<Theory>]
[<InlineData(typeof<R1>, null, null)>]
[<InlineData(typeof<C1A_R1>, typeof<R1>, null)>]
[<InlineData(typeof<C1B_R1_R2>, typeof<R1>, typeof<R2>)>]
[<InlineData(typeof<C1C_R1_R1>, typeof<R1>, null)>]
let ``getDepTypes: -> ctor args`` clas (expDep1:Type) (expDep2:Type) =
    let exp = [expDep1; expDep2] |> List.filter (fun x -> x <> null)

    let res = getDepTypes clas

    res |> should equal exp

[<Fact>]
let ``getDepTypes: multiple ctors -> error``() =
    (fun () -> getDepTypes typeof<MultiCtors>) |> assertInvalidOp

// findReg

let findRegCases =
    [
        [(typeof<R1>, r1Reg);];
        [(typeof<IR1>, r1Reg); (typeof<R1>, r1Reg); (typeof<R2>, r2Reg)]
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``findReg: -> matching reg`` case =
    let map = findRegCases.[case] |> dict

    let res = findReg map typeof<R1>

    res |> should equal r1Reg

let findRegErrorCases =
    [
        [(typeof<R1>, r1Reg);];
        [(typeof<IR1>, r1Reg); (typeof<R1>, r1Reg); (typeof<R2>, r2Reg)]
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``findReg: no matching reg -> error`` case =
    let map = findRegErrorCases.[case] |> dict

    (fun () -> findReg map typeof<IR2_1>) |> assertInvalidOp

// BuildDependencyGraph

let getRegCopies() =
    [
        { r1Reg with classType = r1Reg.classType };
        { r2Reg with classType = r2Reg.classType };
        { c1aReg with classType = c1aReg.classType };
        { c1bReg with classType = c1bReg.classType };
        { c1cReg with classType = c1cReg.classType }
    ]

let buildDependencyGraphCases =
    [
        (r1Reg.classType, []);
        (r2Reg.classType, []);
        (c1aReg.classType, [r1Reg]);
        (c1bReg.classType, [r1Reg; r2Reg]);
        (c1cReg.classType, [r1Reg])
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
[<InlineData(4)>]
let ``BuildDependencyGraph: -> deps filled`` case =
    let (regClass, expDeps) = buildDependencyGraphCases.[case]
    let regs = getRegCopies()

    BuildDependencyGraph regs

    regs |> List.find (fun x -> x.classType = regClass) |> (fun x -> x.dependencies) |> should equal expDeps
