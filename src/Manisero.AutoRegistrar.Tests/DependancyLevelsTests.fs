module DependancyLevelsTests

open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open DependancyLevels

// test data

let r1Reg = { defaultRegistration with classType = typeof<R1>; }
let r2Reg = { defaultRegistration with classType = typeof<R2>; }
let c1aReg = { defaultRegistration with classType = typeof<C1A_R1>; dependencies = [r1Reg] }
let c1bReg = { defaultRegistration with classType = typeof<C1B_R1_R2>; dependencies = [r1Reg; r2Reg] }
let c1cReg = { defaultRegistration with classType = typeof<C1C_R1_R1>; dependencies = [r1Reg] }
let c2aReg = { defaultRegistration with classType = typeof<C2A_R2_C1C>; dependencies = [r1Reg; c1cReg] }
let selfDepReg = { defaultRegistration with classType = typeof<SelfDependency> }
selfDepReg.dependencies <- [selfDepReg]
let cyclicDep1Reg = { defaultRegistration with classType = typeof<CyclicDependency1> }
let cyclicDep2Reg = { defaultRegistration with classType = typeof<CyclicDependency2> }
cyclicDep1Reg.dependencies <- [cyclicDep2Reg]
cyclicDep2Reg.dependencies <- [cyclicDep1Reg]

// tryAssignLvl

let tryAssignLvlSuccessCases =
    [
        ([], 0);
        ([{ defaultRegistration with dependancyLevel = Some 0 }], 1);
        ([{ defaultRegistration with dependancyLevel = Some 1 }], 2);
        ([{ defaultRegistration with dependancyLevel = Some 0 }; { defaultRegistration with dependancyLevel = Some 1 }], 2)
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
let ``tryAssignLvl: all deps have lvl -> true, dependancyLevel = highest dep lvl + 1`` case =
    let (deps, expLvl) = tryAssignLvlSuccessCases.[case]
    let reg = { defaultRegistration with dependencies = deps }

    let res = tryAssignLvl reg

    reg.dependancyLevel.Value |> should equal expLvl
    res |> should equal true

let tryAssignLvlFailureCases =
    [
        [{ defaultRegistration with dependancyLevel = None }];
        [{ defaultRegistration with dependancyLevel = Some 1 }; { defaultRegistration with dependancyLevel = None }]
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``tryAssignLvl: some deps don't have lvl -> false, dependancyLevel None`` case =
    let deps = tryAssignLvlFailureCases.[case]
    let reg = { defaultRegistration with dependencies = deps }

    let res = tryAssignLvl reg

    reg.dependancyLevel |> should equal None
    res |> should equal false

// AssignDependancyLevels

let assertRegDepLvl expLvl reg =
    reg.dependancyLevel |> should equal (Some expLvl)

let assignDependancyLevelsCases =
    [
        (r1Reg.classType, 0);
        (c1aReg.classType, 1);
        (c1bReg.classType, 1);
        (c1cReg.classType, 1);
        (c2aReg.classType, 2)
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
[<InlineData(4)>]
let ``AssignDependancyLevels: -> dependancyLevel set`` case =
    let (regClass, expLvl) = assignDependancyLevelsCases.[case]

    let r1Reg = { r1Reg with classType = r1Reg.classType }
    let r2Reg = { r2Reg with classType = r2Reg.classType }
    let c1aReg = { c1aReg with dependencies = [r1Reg] }
    let c1bReg = { c1bReg with dependencies = [r1Reg; r2Reg] }
    let c1cReg = { c1cReg with dependencies = [r1Reg] }
    let c2aReg = { c2aReg with dependencies = [r1Reg; c1cReg] }

    let regs = List.rev [ r1Reg; r2Reg; c1aReg; c1bReg; c1cReg; c2aReg] // Reversed to force more than one iteration over regs

    AssignDependancyLevels regs

    regs |> List.find (fun x -> x.classType = regClass) |> assertRegDepLvl expLvl

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
