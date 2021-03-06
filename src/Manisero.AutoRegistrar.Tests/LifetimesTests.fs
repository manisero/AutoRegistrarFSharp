﻿module Manisero.AutoRegistrar.LifetimesTests

open System
open System.Collections.Generic
open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open Internal.Lifetimes

// test data

let r1Reg = new Registration(typeof<R1>, DependancyLevel = Nullable 0)
let r1RegRes = new Registration(r1Reg.ClassType, DependancyLevel = r1Reg.DependancyLevel, Lifetime = Nullable 1)
let r2Reg = new Registration(typeof<R2>, DependancyLevel = Nullable 0)
let r2RegRes = new Registration(r2Reg.ClassType, DependancyLevel = r2Reg.DependancyLevel, Lifetime = Nullable 2)
let c1aReg = new Registration(typeof<C1A_R1>, Dependencies = new List<Registration>([r1Reg]), DependancyLevel = Nullable 1)
let c1bReg = new Registration(typeof<C1B_R1_R2>, Dependencies = new List<Registration>([r1Reg; r2RegRes]), DependancyLevel = Nullable 1)
let c1cReg = new Registration(typeof<C1C_R1_R1>, Dependencies = new List<Registration>([r1Reg]), DependancyLevel = Nullable 1)
let c1cRegRes = new Registration(c1cReg.ClassType, Dependencies = c1cReg.Dependencies, DependancyLevel = c1cReg.DependancyLevel, Lifetime = Nullable 4)
let c2aReg = new Registration(typeof<C2A_R2_C1C>, Dependencies = new List<Registration>([r1Reg; c1cRegRes]), DependancyLevel = Nullable 2)

// resolveLifetime

[<Theory>]
[<InlineData(1)>]
[<InlineData(2)>]
let ``resolveLifetime: already resolved -> longestLifetime stays intact`` lifetime =
    let reg = new Registration(null, Lifetime = Nullable lifetime)

    resolveLifetime reg

    reg.Lifetime |> assertEqualsNullable (Nullable lifetime)

[<Fact>]
let ``resolveLifetime: no deps -> lifetime = longestLifetime`` = 
    let reg = new Registration(null, Dependencies = new List<Registration>())

    resolveLifetime reg

    reg.Lifetime |> should equal Registration.LongestLifetime

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
    let reg = new Registration(null, Dependencies = new List<Registration>(deps))

    resolveLifetime reg

    reg.Lifetime |> assertEqualsNullable (Nullable exp)

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
    let reg = new Registration(r1Reg.ClassType, Dependencies = new List<Registration>(deps), DependancyLevel = r1Reg.DependancyLevel)
    
    (fun () -> resolveLifetime reg) |> assertInvalidOp

// ResolveLifetimes

let ResolveLifetimesCases =
    [
        (r1Reg.ClassType, Nullable Registration.LongestLifetime);
        (r2RegRes.ClassType, r2RegRes.Lifetime);
        (c1aReg.ClassType, Nullable Registration.LongestLifetime);
        (c1bReg.ClassType, r2RegRes.Lifetime);
        (c1cRegRes.ClassType, c1cRegRes.Lifetime);
        (c2aReg.ClassType, c1cRegRes.Lifetime);
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

    let r1Reg = new Registration(r1Reg.ClassType, DependancyLevel = r1Reg.DependancyLevel)
    let r2RegRes = new Registration(r2RegRes.ClassType, DependancyLevel = r2RegRes.DependancyLevel, Lifetime = r2RegRes.Lifetime)
    let c1aReg = new Registration(c1aReg.ClassType, Dependencies = new List<Registration>([r1Reg]), DependancyLevel = c1aReg.DependancyLevel)
    let c1bReg = new Registration(c1bReg.ClassType, Dependencies = new List<Registration>([r1Reg; r2RegRes]), DependancyLevel = c1bReg.DependancyLevel)
    let c1cRegRes = new Registration(c1cRegRes.ClassType, Dependencies = new List<Registration>([r1Reg]), DependancyLevel = c1cRegRes.DependancyLevel, Lifetime = c1cRegRes.Lifetime)
    let c2aReg = new Registration(c2aReg.ClassType, Dependencies = new List<Registration>([r1Reg; c1cRegRes]), DependancyLevel = c2aReg.DependancyLevel)

    let regs = List.rev [ c2aReg; c1cRegRes; r2RegRes; c1aReg; r1Reg; c1bReg; ] // Random order to force proper order of lifetime resolution

    let res = ResolveLifetimes regs

    res |> should equal regs
    regs |> List.find (fun x -> x.ClassType = regClass) |> (fun x -> x.Lifetime) |> should equal expLifetime

let ResolveLifetimesErrorCases =
    [
        [new Registration(r1Reg.ClassType)];
        [r1Reg; new Registration(r2Reg.ClassType)];
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``ResolveLifetimes: no dependancyLevel -> error`` case = 
    let regs = ResolveLifetimesErrorCases.[case]
    
    (fun () -> ResolveLifetimes regs) |> assertInvalidOp
