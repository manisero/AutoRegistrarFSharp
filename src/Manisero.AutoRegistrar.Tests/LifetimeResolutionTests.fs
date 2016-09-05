module LifetimeResolutionTests

open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.TestClasses
open LifetimeResolution

let noDepsReg = { defaultRegistration with classType = typedefof<NoDependencies> }
let noDepsReg_lifetime = { noDepsReg with lifetime = Some 3 }

[<Fact>]
let ``no dependencies -> 1``() = 
    let result = resolveLifetime noDepsReg []

    result.classType |> should equal noDepsReg.classType
    result.lifetime |> should equal longestLifetime
