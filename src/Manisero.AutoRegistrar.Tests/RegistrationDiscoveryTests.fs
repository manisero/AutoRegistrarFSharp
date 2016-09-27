module Manisero.AutoRegistrar.RegistrationDiscoveryTests

open System
open Xunit
open FsUnit.Xunit
open Domain
open TestsHelpers
open Manisero.AutoRegistrar.TestClasses
open Manisero.AutoRegistrar.TestClasses2
open RegistrationDiscovery

// test data

let intInters = None
let intLifetime = None
let intReg = { defaultRegistration with classType = typeof<int>; interfaceTypes = intInters; lifetime = intLifetime }
let r1Inters = Some []
let r1Lifetime = None
let r1Reg = { defaultRegistration with classType = typeof<R1>; interfaceTypes = r1Inters; lifetime = r1Lifetime }
let r2Inters = Some [typeof<IR2_1>; typeof<IR2_2>]
let r2Lifetime = Some 3
let r2Reg = { defaultRegistration with classType = typeof<R2>; interfaceTypes = r2Inters; lifetime = r2Lifetime }

let testAss = typeof<R1>.Assembly
let initRegs = [intReg; r1Reg; r2Reg]
let typeFilter (typ:Type) = typ.FullName.Contains("1")

// DiscoverRegistrations

[<Fact>]
let ``no initRegs, no filter, single assembly -> concrete classes from assembly`` () =
    let res = DiscoverRegistrations [] None [testAss]

    let types = res |> List.map (fun x -> x.classType)
    types |> assertContains [typeof<R1>; typeof<R2>; typeof<C1A_R1>; typeof<NoInters>]
    types |> assertNotContains [typeof<IR2_1>; typeof<R2_Base>; typeof<IR2_Base>]

[<Fact>]
let ``some initRegs -> contains all init types``() =
    let res = DiscoverRegistrations initRegs None [testAss]

    res |> List.map (fun x -> x.classType) |> assertContains [intReg.classType; r1Reg.classType; r2Reg.classType]

[<Fact>]
let ``some initRegs -> no duplicates``() =
    let res = DiscoverRegistrations initRegs None [testAss]

    res |> Seq.distinctBy (fun x -> x.classType) |> Seq.length |> should equal res.Length

let assertIntersAndLife typ (expInters:Type list option) (expLife:int option) regs =
    let reg = regs |> List.find (fun x -> x.classType = typ)
    reg.interfaceTypes |> assertEqualsOption expInters
    reg.lifetime |> assertEqualsOption expLife

[<Fact>]
let ``some initRegs -> interfaceTypes and lifetimes not overriden``() =
    let res = DiscoverRegistrations initRegs None [testAss]

    res |> assertIntersAndLife intReg.classType intInters intLifetime
    res |> assertIntersAndLife r1Reg.classType r1Inters r1Lifetime
    res |> assertIntersAndLife r2Reg.classType r2Inters r2Lifetime

[<Fact>]
let ``some filter -> types filtered``() =
    let res = DiscoverRegistrations [] (Some typeFilter) [testAss]
    
    let types = res |> List.map (fun x -> x.classType)
    types |> should contain typeof<R1>
    types |> should not' (contain typeof<R2>)
    types |> should not' (contain typeof<NoInters>)

[<Fact>]
let ``some initRegs, some filter -> filter not applied to init types``() =
    let res = DiscoverRegistrations initRegs (Some typeFilter) [testAss]
    
    let types = res |> List.map (fun x -> x.classType)
    types |> assertContains [typeof<R1>; typeof<R2>]
    types |> should not' (contain typeof<NoInters>)

[<Fact>]
let ``multiple assemblies -> types from all assemblies`` () =
    let assemblies = [typeof<TestClass>.Assembly; testAss]

    let res = DiscoverRegistrations [] None assemblies

    let types = res |> List.map (fun x -> x.classType)
    types |> assertContains [typeof<TestClass>; typeof<R1>; typeof<R2>]
