module Manisero.AutoRegistrar.SharedTests

open System.Linq
open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open Shared

// test data

let r1Reg = new Registration(typeof<R1>, InterfaceTypes = Some [typeof<IR1>])
let r2Reg = new Registration(typeof<R2>, InterfaceTypes = Some [typeof<R2_Base>; typeof<IR2_1>; typeof<IR2_2>])
let c1aReg = new Registration(typeof<C1A_R1>, InterfaceTypes = Some [typeof<IC1A_R1>])
let multiImpl1Reg = new Registration(typeof<MultiImpl1>, InterfaceTypes = Some [typeof<IMultiImpls>])
let multiImpl2Reg = new Registration(typeof<MultiImpl2>, InterfaceTypes = Some [typeof<IMultiImpls>])

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
    res.[inter].ClassType |> should equal impl

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
    let regs = buildTypeToRegMapErrorCases.[case] |> List.map (fun (clas, inters) -> new Registration(clas, InterfaceTypes = Some inters))

    (fun () -> buildTypeToRegMap regs) |> assertInvalidOp
