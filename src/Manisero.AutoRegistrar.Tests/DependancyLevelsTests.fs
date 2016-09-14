module DependancyLevelsTests

open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open DependancyLevels

// test data

let r1Reg = { defaultRegistration with classType = typeof<R1>; interfaceTypes = [typeof<IR1>] }
let r2Reg = { defaultRegistration with classType = typeof<R2>; interfaceTypes = [typeof<R2_Base>; typeof<IR2_1>; typeof<IR2_2>] }
let c1aReg = { defaultRegistration with classType = typeof<C1A_R1>; interfaceTypes = [typeof<IC1A_R1>]; dependencies = [r1Reg] }
let c1bReg = { defaultRegistration with classType = typeof<C1B_R1_R2>; interfaceTypes = [typeof<IC1B_R1_R2>]; dependencies = [r1Reg; r2Reg] }
let c1cReg = { defaultRegistration with classType = typeof<C1C_R1_R1>; interfaceTypes = [typeof<IC1C_R1_R1>]; dependencies = [r1Reg] }
let c2aReg = { defaultRegistration with classType = typeof<C2A_R2_C1C>; interfaceTypes = [typeof<IC2A_R2_C1C>]; dependencies = [r1Reg; c1cReg] }

// tryAssignLvl

let tryAssignLvlCases =
    [
        ([{ defaultRegistration with dependancyLevel = Some 0 }], 1)
    ]

[<Theory>]
[<InlineData(0)>]
let ``tryAssignLvl: all deps have lvl -> true, dependancyLevel set`` case =
    let (deps, expLvl) = tryAssignLvlCases.[case]
    let reg = { defaultRegistration with dependencies = deps }

    let res = tryAssignLvl reg

    reg.dependancyLevel.Value |> should equal expLvl
    res |> should equal true

// AssignDependancyLevels

let assertRegDepLvl expLvl reg =
    reg.dependancyLevel |> should equal expLvl

let assignDependancyLevelsCases =
    [
        (r1Reg.classType, 0);
        (c1aReg.classType, 1);
        (c1bReg.classType, 1);
        (c1cReg.classType, 1)
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
let ``AssignDependancyLevels: -> dependancyLevel set`` case =
    let (regClass, expLvl) = assignDependancyLevelsCases.[case]

    let regCopies = 
        [
            { r1Reg with classType = r1Reg.classType };
            { r2Reg with classType = r2Reg.classType };
            { c1aReg with classType = c1aReg.classType };
            { c1bReg with classType = c1bReg.classType };
            { c1cReg with classType = c1cReg.classType }
        ]

    AssignDependancyLevels regCopies

    regCopies |> List.find (fun x -> x.classType = regClass) |> assertRegDepLvl expLvl
