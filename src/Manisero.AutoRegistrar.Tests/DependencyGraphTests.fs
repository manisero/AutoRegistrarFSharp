module Manisero.AutoRegistrar.DependencyGraphTests

open System
open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open DependencyGraph

// test data

let r1Reg = new Registration(typeof<R1>, InterfaceTypes = Some [typeof<IR1>])
let r2Reg = new Registration(typeof<R2>, InterfaceTypes = Some [typeof<R2_Base>; typeof<IR2_1>; typeof<IR2_2>])
let c1aReg = new Registration(typeof<C1A_R1>, InterfaceTypes = Some [typeof<IC1A_R1>])
let c1bReg = new Registration(typeof<C1B_R1_R2>, InterfaceTypes = Some [typeof<IC1B_R1_R2>])
let c1cReg = new Registration(typeof<C1C_R1_R1>, InterfaceTypes = Some [typeof<IC1C_R1_R1>])

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
let buildDependencyGraphCases =
    [
        (r1Reg.ClassType, []);
        (r2Reg.ClassType, []);
        (c1aReg.ClassType, [r1Reg.ClassType]);
        (c1bReg.ClassType, [r1Reg.ClassType; r2Reg.ClassType]);
        (c1cReg.ClassType, [r1Reg.ClassType])
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
[<InlineData(4)>]
let ``BuildDependencyGraph: -> deps filled`` case =
    let regs =
        [
            new Registration(r1Reg.ClassType, InterfaceTypes = r1Reg.InterfaceTypes);
            new Registration(r2Reg.ClassType, InterfaceTypes = r2Reg.InterfaceTypes);
            new Registration(c1aReg.ClassType, InterfaceTypes = c1aReg.InterfaceTypes);
            new Registration(c1bReg.ClassType, InterfaceTypes = c1bReg.InterfaceTypes);
            new Registration(c1cReg.ClassType, InterfaceTypes = c1cReg.InterfaceTypes)
        ]
    let (regClass, expDepClasses) = buildDependencyGraphCases.[case]

    let res = BuildDependencyGraph regs

    res |> should equal regs
    regs |> List.find (fun x -> x.ClassType = regClass) |> (fun x -> x.Dependencies) |> List.map (fun x -> x.ClassType) |> should equal expDepClasses
    