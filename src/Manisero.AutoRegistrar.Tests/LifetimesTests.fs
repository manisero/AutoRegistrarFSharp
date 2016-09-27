module LifetimesTests

open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open Lifetimes

// test data

let r1Reg = { defaultRegistration with classType = typeof<R1>; dependancyLevel = Some 0 }
let r1RegRes = { r1Reg with lifetime = Some 1 }
let r2Reg = { defaultRegistration with classType = typeof<R2>; dependancyLevel = Some 0; }
let r2RegRes = { r2Reg with lifetime = Some 2 }
let c1aReg = { defaultRegistration with classType = typeof<C1A_R1>; dependencies = [r1Reg]; dependancyLevel = Some 1 }
let c1bReg = { defaultRegistration with classType = typeof<C1B_R1_R2>; dependencies = [r1Reg; r2RegRes]; dependancyLevel = Some 1; }
let c1cReg = { defaultRegistration with classType = typeof<C1C_R1_R1>; dependencies = [r1Reg]; dependancyLevel = Some 1; }
let c1cRegRes = { c1cReg with lifetime = Some 4 }
let c2aReg = { defaultRegistration with classType = typeof<C2A_R2_C1C>; dependencies = [r1Reg; c1cRegRes]; dependancyLevel = Some 2 }

// resolveLifetime

[<Theory>]
[<InlineData(1)>]
[<InlineData(2)>]
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
[<InlineData(1)>]
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
    let reg = { r1Reg with dependencies = deps }
    
    (fun () -> resolveLifetime reg) |> assertInvalidOp

// ResolveLifetimes

let ResolveLifetimesCases =
    [
        (r1Reg.classType, longestLifetime);
        (r2RegRes.classType, r2RegRes.lifetime);
        (c1aReg.classType, longestLifetime);
        (c1bReg.classType, r2RegRes.lifetime);
        (c1cRegRes.classType, c1cRegRes.lifetime);
        (c2aReg.classType, c1cRegRes.lifetime);
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
[<InlineData(4)>]
[<InlineData(5)>]
let ``ResolveLifetimes: -> lifetime set`` case = 
    let (regClass, expLifetime) = ResolveLifetimesCases.[case]

    let r1Reg = { r1Reg with classType = r1Reg.classType; }
    let r2RegRes = { r2RegRes with classType = r2RegRes.classType }
    let c1aReg = { c1aReg with dependencies = [r1Reg] }
    let c1bReg = { c1bReg with dependencies = [r1Reg; r2RegRes] }
    let c1cRegRes = { c1cRegRes with dependencies = [r1Reg] }
    let c2aReg = { c2aReg with dependencies = [r1Reg; c1cRegRes] }

    let regs = List.rev [ c2aReg; c1cRegRes; r2RegRes; c1aReg; r1Reg; c1bReg; ] // Random order to force proper order of lifetime resolution

    let res = ResolveLifetimes regs

    res |> should equal regs
    regs |> List.find (fun x -> x.classType = regClass) |> (fun x -> x.lifetime) |> should equal expLifetime

let ResolveLifetimesErrorCases =
    [
        [{ r1Reg with dependancyLevel = None }];
        [r1Reg; { r2Reg with dependancyLevel = None }];
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``ResolveLifetimes: no dependancyLevel -> error`` case = 
    let regs = ResolveLifetimesErrorCases.[case]
    
    (fun () -> ResolveLifetimes regs) |> assertInvalidOp
