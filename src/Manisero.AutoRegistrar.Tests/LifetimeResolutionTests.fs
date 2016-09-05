module LifetimeResolutionTests

open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.TestClasses
open LifetimeResolution

let NoDepsReg = { defaultRegistration with classType = typedefof<NoDependencies> }

[<Fact>]
let ``no dependencies -> 1``() = 
    let result = resolveLifetime NoDepsReg []

    result.lifetime |> should equal longestLifetime
