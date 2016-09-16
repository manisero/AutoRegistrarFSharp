module LifetimeResolutionTests

open System
open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open LifetimeResolution

// test data

let r1Reg = { defaultRegistration with classType = typeof<R1>; }
let r1RegRes = { defaultRegistration with classType = typeof<R1>; lifetime = Some 1 }
let r2Reg = { defaultRegistration with classType = typeof<R2>; }
let r2RegRes = { defaultRegistration with classType = typeof<R2>; lifetime = Some 2 }
let c1aReg = { defaultRegistration with classType = typeof<C1A_R1>; dependencies = [r1Reg] }
let c1bReg = { defaultRegistration with classType = typeof<C1B_R1_R2>; dependencies = [r1Reg; r2Reg] }
let c1cReg = { defaultRegistration with classType = typeof<C1C_R1_R1>; dependencies = [r1Reg] }
let c2aReg = { defaultRegistration with classType = typeof<C2A_R2_C1C>; dependencies = [r1Reg; c1cReg] }

// tryResolve

let tryResolveAlreadyResolvedCases =
    [
        (r1RegRes, r1RegRes.lifetime);
        (r2RegRes, r2RegRes.lifetime)
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``already resolved -> longestLifetime stays intact`` case =
    let (reg, exp) = tryResolveAlreadyResolvedCases.[case]

    tryResolve reg

    reg.lifetime |> should equal exp


[<Fact>]
let ``no deps -> lifetime = longestLifetime``() = 
    raise (NotImplementedException())

[<Fact>]
let ``deps -> lifetime derived from shortest living dep``() = 
    raise (NotImplementedException())
