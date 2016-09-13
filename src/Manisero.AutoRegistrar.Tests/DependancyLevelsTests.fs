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
