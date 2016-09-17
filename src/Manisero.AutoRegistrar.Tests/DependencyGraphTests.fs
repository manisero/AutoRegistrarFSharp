module DependencyGraphTests

open System
open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open DependencyGraph

// test data

let r1Reg = { defaultRegistration with classType = typeof<R1>; interfaceTypes = [typeof<IR1>] }
let r2Reg = { defaultRegistration with classType = typeof<R2>; interfaceTypes = [typeof<R2_Base>; typeof<IR2_1>; typeof<IR2_2>] }
let c1aReg = { defaultRegistration with classType = typeof<C1A_R1>; interfaceTypes = [typeof<IC1A_R1>] }
let c1bReg = { defaultRegistration with classType = typeof<C1B_R1_R2>; interfaceTypes = [typeof<IC1B_R1_R2>] }
let c1cReg = { defaultRegistration with classType = typeof<C1C_R1_R1>; interfaceTypes = [typeof<IC1C_R1_R1>] }

// buildInterToImplMap

let ``buildInterToImplMap: single impl -> impl for each inter`` case = "TODO"

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
        (typeof<R1>, [r1Reg], r1Reg);
        (typeof<R2>, [r1Reg; r2Reg], r2Reg);
        (typeof<R2_Base>, [r1Reg; r2Reg], r2Reg);
        (typeof<IR2_1>, [r1Reg; r2Reg], r2Reg)
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
let ``findReg: -> matching reg`` case =
    let (typ, regs, exp) = findRegCases.[case]

    let res = findReg regs typ

    res |> should equal exp

let findRegErrorCases =
    [
        (typeof<R2>, [r1Reg]);
        (typeof<R2_Base>, [r1Reg]);
        (typeof<IR2_1>, [r1Reg])
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
let ``findReg: no matching reg -> error`` case =
    let (typ, regs) = findRegErrorCases.[case]

    (fun () -> findReg regs typ) |> assertInvalidOp

// BuildDependencyGraph

let getRegCopies() =
    [
        { r1Reg with classType = r1Reg.classType };
        { r2Reg with classType = r2Reg.classType };
        { c1aReg with classType = c1aReg.classType };
        { c1bReg with classType = c1bReg.classType };
        { c1cReg with classType = c1cReg.classType }
    ]

let assertRegDeps expDeps reg =
    reg.dependencies |> should equal expDeps

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

    regs |> List.find (fun x -> x.classType = regClass) |> assertRegDeps expDeps
