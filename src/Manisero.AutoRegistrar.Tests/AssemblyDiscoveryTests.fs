module AssemblyDiscoveryTests

open System
open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.TestClasses
open Manisero.AutoRegistrar.TestClasses2
open AssemblyDiscovery

// DiscoverAssemblies

[<Fact>]
let ``TestClasses -> TestClasses and ReferencedByTestClassesOnly``() =
    null
