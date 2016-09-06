module SampleTests

open Xunit
open FsUnit.Xunit

[<Fact>]
let ``passing test``() = 
    true |> should equal true

[<Theory>]
[<InlineData(true)>]
let ``theory``(input) = 
    input |> should equal true
