module Manisero.AutoRegistrar.DependancyLevelsTests

open System
open System.Collections.Generic
open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open DependancyLevels

// test data

let r1Reg = new Registration(typeof<R1>)
let r2Reg = new Registration(typeof<R2>)
let c1aReg = new Registration(typeof<C1A_R1>, Dependencies = new List<Registration>([r1Reg]))
let c1bReg = new Registration(typeof<C1B_R1_R2>, Dependencies = new List<Registration>([r1Reg; r2Reg]))
let c1cReg = new Registration(typeof<C1C_R1_R1>, Dependencies = new List<Registration>([r1Reg]))
let c2aReg = new Registration(typeof<C2A_R2_C1C>, Dependencies = new List<Registration>([r1Reg; c1cReg]))
let selfDepReg = new Registration(typeof<SelfDependency>)
selfDepReg.Dependencies <- new List<Registration>([selfDepReg])
let cyclicDep1Reg = new Registration(typeof<CyclicDependency1>)
let cyclicDep2Reg = new Registration(typeof<CyclicDependency2>)
cyclicDep1Reg.Dependencies <- new List<Registration>([cyclicDep2Reg])
cyclicDep2Reg.Dependencies <- new List<Registration>([cyclicDep1Reg])

// tryAssignLvl

let tryAssignLvlSuccessCases =
    [
        ([], 0);
        ([new Registration(null, DependancyLevel = Nullable 0)], 1);
        ([new Registration(null, DependancyLevel = Nullable 1)], 2);
        ([new Registration(null, DependancyLevel = Nullable 0); new Registration(null, DependancyLevel = Nullable 1)], 2)
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
let ``tryAssignLvl: all deps have lvl -> true, dependancyLevel = highest dep lvl + 1`` case =
    let (deps, expLvl) = tryAssignLvlSuccessCases.[case]
    let reg = new Registration(null, Dependencies = new List<Registration>(deps))

    let res = tryAssignLvl reg

    reg.DependancyLevel.Value |> should equal expLvl
    res |> should equal true

let tryAssignLvlFailureCases =
    [
        [new Registration(null, DependancyLevel = Nullable())];
        [new Registration(null, DependancyLevel = Nullable 1); new Registration(null, DependancyLevel = Nullable())]
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``tryAssignLvl: some deps don't have lvl -> false, dependancyLevel None`` case =
    let deps = tryAssignLvlFailureCases.[case]
    let reg = new Registration(null, Dependencies = new List<Registration>(deps))

    let res = tryAssignLvl reg

    reg.DependancyLevel |> should equal None
    res |> should equal false

// AssignDependancyLevels

let assignDependancyLevelsCases =
    [
        (r1Reg.ClassType, 0);
        (c1aReg.ClassType, 1);
        (c1bReg.ClassType, 1);
        (c1cReg.ClassType, 1);
        (c2aReg.ClassType, 2)
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
[<InlineData(4)>]
let ``AssignDependancyLevels: -> dependancyLevel set`` case =
    let (regClass, expLvl) = assignDependancyLevelsCases.[case]

    let r1Reg = new Registration(r1Reg.ClassType)
    let r2Reg = new Registration(r2Reg.ClassType)
    let c1aReg = new Registration(c1aReg.ClassType, Dependencies = new List<Registration>([r1Reg]))
    let c1bReg = new Registration(c1bReg.ClassType, Dependencies = new List<Registration>([r1Reg; r2Reg]))
    let c1cReg = new Registration(c1cReg.ClassType, Dependencies = new List<Registration>([r1Reg]))
    let c2aReg = new Registration(c2aReg.ClassType, Dependencies = new List<Registration>([r1Reg; c1cReg]))

    let regs = List.rev [ c2aReg; c1cReg; r2Reg; c1aReg; r1Reg; c1bReg; ] // Random order to force more than one iteration over regs

    let res = AssignDependancyLevels regs

    res |> should equal regs
    regs |> List.find (fun x -> x.ClassType = regClass) |> (fun x -> x.DependancyLevel) |> assertEqualsNullable (Nullable expLvl)

let assignDependancyLevelsErrorCases =
    [
        [selfDepReg];
        [cyclicDep1Reg; cyclicDep2Reg]
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``AssignDependancyLevels: cyclic dependency -> error`` case =
    let regs = assignDependancyLevelsErrorCases.[case]

    (fun () -> AssignDependancyLevels regs) |> assertInvalidOp
