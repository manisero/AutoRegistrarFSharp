module LifetimeResolutionTests

open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
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

// resolveLifetime

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``resolveLifetime: already resolved -> longestLifetime stays intact`` lifetime =
    let reg = { defaultRegistration with lifetime = Some lifetime }

    resolveLifetime reg

    reg.lifetime |> should equal (Some lifetime)

[<Fact>]
let ``resolveLifetime: no deps -> lifetime = longestLifetime`` = 
    let reg = { defaultRegistration with dependencies = [] }

    resolveLifetime reg

    reg.lifetime |> should equal longestLifetime

let resolveLifetimeDepsCases =
    [
        ([r1RegRes], 1);
        ([r1RegRes; r2RegRes], 2);
    ]

[<Theory>]
[<InlineData(0)>]
let ``resolveLifetime: deps -> lifetime derived from shortest living dep`` case = 
    let (deps, exp) = resolveLifetimeDepsCases.[case]
    let reg = { defaultRegistration with dependencies = deps }

    resolveLifetime reg

    reg.lifetime |> should equal (Some exp)

let resolveLifetimeErrorCases =
    [
        [r1Reg];
        [r1RegRes; r2Reg]
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``resolveLifetime: dep with no lifetime -> error`` case = 
    let deps = resolveLifetimeErrorCases.[case]
    let reg = { defaultRegistration with dependencies = deps }

    (fun () -> resolveLifetime reg) |> assertInvalidOp
