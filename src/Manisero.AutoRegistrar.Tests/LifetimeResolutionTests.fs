module LifetimeResolutionTests

open Xunit
open FsUnit.Xunit
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
