module DependencyGraphTests

open System
open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open DependencyGraph

// test data

let noDepsReg = { defaultRegistration with classType = typeof<NoDependencies> }
let depOfNoDepsReg =  { defaultRegistration with classType = typeof<DependantOf_NoDependencies> }

// helpers

let assertFails exceptionType action =
    (fun () -> action() |> ignore) |> should throw exceptionType

// BuildDependencyGraph

[<Fact>]
let ``no ctor args -> no deps``() =
    let reg = { noDepsReg with classType = noDepsReg.classType }
    BuildDependencyGraph [reg]

    reg.dependencies |> should equal List.empty<Registration>

// getDependencyTypes

[<Theory>]
[<InlineData(typeof<NoDependencies>, null, null)>]
let ``getDepTypes: -> ctor args`` clas (expDep1:Type) (expDep2:Type) =
    let res = getDepTypes typeof<NoDependencies>

    let exp = [|expDep1; expDep2|] |> Array.filter (fun x -> x <> null)
    res |> should equal exp
