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

// AssignDependancyLevels

let getRegCopies() =
    [
        { r1Reg with classType = r1Reg.classType };
        { r2Reg with classType = r2Reg.classType };
        { c1aReg with classType = c1aReg.classType };
        { c1bReg with classType = c1bReg.classType };
        { c1cReg with classType = c1cReg.classType }
    ]

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
    let regs = getRegCopies()

    AssignDependancyLevels regs

    regs |> List.find (fun x -> x.classType = regClass) |> assertRegDepLvl expLvl
