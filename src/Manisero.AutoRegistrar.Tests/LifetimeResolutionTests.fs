module LifetimeResolutionTests

open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.TestClasses
open LifetimeResolution

[<Fact>]
let ``passing test``() = 
    true |> should equal true

[<Fact>]
let ``failing test``() = 
    true |> should equal false

[<Theory>]
[<InlineData(true)>]
[<InlineData(false)>]
let ``theory``(input) = 
    input |> should equal true

let NoDepsReg = { defaultRegistration with classType = typedefof<NoDependencies> }

[<Fact>]
let ``no dependencies -> 1``() = 
    let result = resolveLifetime NoDepsReg []

    result.lifetime |> should equal longestLifetime
