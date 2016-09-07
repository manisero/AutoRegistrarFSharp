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

// BuildDependencyGraph

[<Fact>]
let ``no ctor args -> no deps``() =
    let reg = { noDepsReg with classType = noDepsReg.classType }
    BuildDependencyGraph [reg]

    reg.dependencies |> should equal List.empty<Registration>

// getDependencyTypes

