module Manisero.AutoRegistrar.AssemblyDiscoveryTests

open System.Reflection
open Xunit
open FsUnit.Xunit
open TestsHelpers
open Manisero.AutoRegistrar.TestClasses
open AssemblyDiscovery

// DiscoverAssemblies

let root = typeof<R1>.Assembly
let assRefByTestClasses = ReferencedByTestClassesOnly.Assembly
let mscorlibAss = typeof<string>.Assembly

[<Fact>]
let ``DiscoverAssemblies: -> root ass and referenced asses``() =
    let res = DiscoverAssemblies None root
    
    res |> assertContains [root; assRefByTestClasses; mscorlibAss]

[<Fact>]
let ``DiscoverAssemblies: -> no duplicates``() =
    let res = DiscoverAssemblies None root
    
    res |> Seq.distinct |> Seq.length |> should equal res.Length

let discoverAssembliesFilterCases =
    [
        ((fun (x:Assembly) -> x <> root), [root], [assRefByTestClasses; mscorlibAss]);
        ((fun (x:Assembly) -> not (x.FullName.Contains "mscorlib")), [mscorlibAss], [root; assRefByTestClasses])
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``DiscoverAssemblies: filter -> filtered`` case =
    let (filter, rejectedAsses, acceptedAsses) = discoverAssembliesFilterCases.[case]

    let res = DiscoverAssemblies (Some filter) root
    
    res |> assertNotContains rejectedAsses
    res |> assertContains acceptedAsses
