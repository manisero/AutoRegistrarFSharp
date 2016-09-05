module LifetimeResolutionTests

open System
open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.TestClasses
open LifetimeResolution

// test data

let noDepsReg = { defaultRegistration with classType = typeof<NoDependencies> }
let noDepsReg_lifetime = { noDepsReg with lifetime = Some 3 }

let depOfNoDepsReg =  { defaultRegistration with classType = typeof<DependantOf_NoDependencies> }

let multiCtorsReg =  { defaultRegistration with classType = typeof<MultipleConstructors> }

// helpers

let assertRegLengths expectedResolvedLength expectedRemainingLength result =
    result.resolvedRegistrations.Length |> should equal expectedResolvedLength
    result.remainingRegistrations.Length |> should equal expectedRemainingLength

let assertReg expectedClassType expectedLifetime reg =
    reg.classType |> should equal expectedClassType
    reg.lifetime |> should equal expectedLifetime

let assertFails exceptionType action =
    (fun () -> action() |> ignore) |> should throw exceptionType

// tests

[<Fact>]
let ``already resolved -> existing registration``() = raise (NotImplementedException())

[<Fact>]
let ``no dependencies -> longestLifetime``() = 
    let result = resolveLifetime noDepsReg [] []

    result |> assertRegLengths 1 0
    List.head result.resolvedRegistrations |> assertReg noDepsReg.classType longestLifetime

[<Fact>]
let ``single dependency -> derived``() = 
    let result = resolveLifetime depOfNoDepsReg [noDepsReg_lifetime] []

    result |> assertRegLengths 2 0
    List.head result.resolvedRegistrations |> assertReg depOfNoDepsReg.classType noDepsReg_lifetime.lifetime

[<Fact>]
let ``multiple constructors -> error``() = 
    (fun () -> resolveLifetime multiCtorsReg [] []) |> assertFails typeof<InvalidOperationException>
