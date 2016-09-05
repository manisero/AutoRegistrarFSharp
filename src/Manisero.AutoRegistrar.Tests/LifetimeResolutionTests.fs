module LifetimeResolutionTests

open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.TestClasses
open LifetimeResolution

// test data

let noDepsReg = { defaultRegistration with classType = typedefof<NoDependencies> }
let noDepsReg_lifetime = { noDepsReg with lifetime = Some 3 }

let depOfNoDepsReg =  { defaultRegistration with classType = typedefof<DependantOf_NoDependencies> }

// helpers

let assertRegLengths expectedResolvedLength expectedRemainingLength output =
    output.resolvedRegistrations.Length |> should equal expectedResolvedLength
    output.remainingRegistrations.Length |> should equal expectedRemainingLength

let assertReg expectedClassType expectedLifetime reg =
    reg.classType |> should equal expectedClassType
    reg.lifetime |> should equal expectedLifetime

// tests

[<Fact>]
let ``no dependencies -> longestLifetime``() = 
    let result = resolveLifetime noDepsReg []

    result |> assertRegLengths 1 0
    List.head result.resolvedRegistrations |> assertReg noDepsReg.classType longestLifetime

[<Fact>]
let ``single dependency -> derived``() = 
    let result = resolveLifetime depOfNoDepsReg [noDepsReg_lifetime]

    result |> assertRegLengths 1 1
    List.head result.resolvedRegistrations |> assertReg depOfNoDepsReg.classType noDepsReg_lifetime.lifetime
