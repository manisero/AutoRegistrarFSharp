module RegistrationDiscoveryTests

open Xunit
open FsUnit.Xunit
open Domain
open TestsHelpers
open Manisero.AutoRegistrar.TestClasses
open RegistrationDiscovery

// DiscoverRegistrations

[<Fact>]
let ``no initRegs, no filter, TestClasses assembly -> concrete classes from TestClasses`` () =
    let res = DiscoverRegistrations [] None [typeof<R1>.Assembly]

    let types = res |> List.map (fun x -> x.classType)
    types |> assertContains [typeof<R1>; typeof<R2>; typeof<C1A_R1>; typeof<NoInters>]
    types |> assertNotContains [typeof<IR2_1>; typeof<R2_Base>; typeof<IR2_Base>]
    
// some TestClasses in initRegs -> no duplicates, init lifetimes not overriden
// no initRegs, filter -> filtered
// initRegs, filter -> filter not applied to initRegs
