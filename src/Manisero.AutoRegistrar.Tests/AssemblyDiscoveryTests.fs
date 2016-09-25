module AssemblyDiscoveryTests

open Xunit
open FsUnit.Xunit
open TestsHelpers
open Manisero.AutoRegistrar.TestClasses
open AssemblyDiscovery

// DiscoverAssemblies

let root = typeof<R1>.Assembly

[<Fact>]
let ``DiscoverAssemblies: -> root ass and referenced asses``() =
    let res = DiscoverAssemblies root None
    
    res |> assertContains [root; ReferencedByTestClassesOnly.Assembly]

[<Fact>]
let ``DiscoverAssemblies: -> no duplicates``() =
    let res = DiscoverAssemblies root None
    
    res |> Seq.distinct |> Seq.length |> should equal res.Length

[<Fact>]
let ``DiscoverAssemblies: filter -> filtered``() =
    null
