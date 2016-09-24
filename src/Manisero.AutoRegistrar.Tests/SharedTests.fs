module SharedTests

open System.Linq
open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open Shared

// test data

let r1Reg = { defaultRegistration with classType = typeof<R1>; interfaceTypes = Some [typeof<IR1>] }
let r2Reg = { defaultRegistration with classType = typeof<R2>; interfaceTypes = Some [typeof<R2_Base>; typeof<IR2_1>; typeof<IR2_2>] }
let c1aReg = { defaultRegistration with classType = typeof<C1A_R1>; interfaceTypes = Some [typeof<IC1A_R1>] }
let multiImpl1Reg = { defaultRegistration with classType = typeof<MultiImpl1>; interfaceTypes = Some [typeof<IMultiImpls>] }
let multiImpl2Reg = { defaultRegistration with classType = typeof<MultiImpl2>; interfaceTypes = Some [typeof<IMultiImpls>] }

// buildTypesSet

let buildTypesSetCases =
    [
        ([r1Reg], [|typeof<R1>; typeof<IR1>|]);
        ([r1Reg; r2Reg], [|typeof<R1>; typeof<IR1>; typeof<R2>; typeof<R2_Base>; typeof<IR2_1>; typeof<IR2_2>|])
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``buildTypesSet: regs -> all types in regs`` case =
    let (regs, expTypes) = buildTypesSetCases.[case]
    
    let res = buildTypesSet regs

    res.ToArray() |> should equal expTypes

let buildTypesSetErrorCases =
    [
        [multiImpl1Reg; multiImpl2Reg]
    ]

[<Theory>]
[<InlineData(0)>]
let ``buildTypesSet: duplicated type -> error`` case =
    let regs = buildTypesSetErrorCases.[case]

    (fun () -> buildTypesSet regs) |> assertInvalidOp

// buildTypeToRegMap

[<Theory>]
[<InlineData(typeof<IR1>, typeof<R1>)>]
[<InlineData(typeof<R2_Base>, typeof<R2>)>]
[<InlineData(typeof<IR2_1>, typeof<R2>)>]
[<InlineData(typeof<IR2_2>, typeof<R2>)>]
[<InlineData(typeof<IC1A_R1>, typeof<C1A_R1>)>]
let ``buildTypeToRegMap: single impl -> inter included`` inter impl =
    let regs = [r1Reg; r2Reg; c1aReg]

    let res = buildTypeToRegMap regs

    res.ContainsKey inter |> should equal true
    res.[inter].classType |> should equal impl

let buildTypeToRegMapErrorCases =
    [
        [(typeof<MultiImpl1>, [typeof<IMultiImpls>]); (typeof<MultiImpl2>, [typeof<IMultiImpls>])];
        [(typeof<R2>, [typeof<IR2_1>]); (typeof<R2>, [typeof<IR2_2>])];
        [(typeof<R2>, [typeof<R2_Base>]); (typeof<R2_Base>, [typeof<IR2_Base>])];
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
let ``buildTypeToRegMap: multiple impls -> error`` case =
    let regs = buildTypeToRegMapErrorCases.[case] |> List.map (fun (clas, inters) -> { defaultRegistration with classType = clas; interfaceTypes = Some inters })

    (fun () -> buildTypeToRegMap regs) |> assertInvalidOp
