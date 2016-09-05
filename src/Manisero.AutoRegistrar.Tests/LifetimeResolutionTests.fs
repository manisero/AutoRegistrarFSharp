module LifetimeResolutionTests

open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.TestClasses
open LifetimeResolution

let noDepsReg = { defaultRegistration with classType = typedefof<NoDependencies> }
let noDepsReg_lifetime = { noDepsReg with lifetime = Some 3 }

let assertReg expectedClassType expectedLifetime reg =
    reg.classType |> should equal expectedClassType
    reg.lifetime |> should equal expectedLifetime

[<Fact>]
let ``no dependencies -> 1``() = 
    let result = resolveLifetime noDepsReg []

    result.resolvedRegistrations.Length |> should equal 1
    result.remainingRegistrations.Length |> should equal 0
    List.head result.resolvedRegistrations |> assertReg noDepsReg.classType longestLifetime
