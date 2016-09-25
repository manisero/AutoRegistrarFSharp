module AssemblyDiscoveryTests

open System
open Xunit
open FsUnit.Xunit
open TestsHelpers
open Manisero.AutoRegistrar.TestClasses
open AssemblyDiscovery

// DiscoverAssemblies

[<Fact>]
let ``TestClasses -> TestClasses and ReferencedByTestClassesOnly``() =
    let root = typeof<R1>.Assembly

    let res = DiscoverAssemblies root None
    
    res |> assertContains [root; ReferencedByTestClassesOnly.Assembly]

// filtering
