module Manisero.AutoRegistrar.RegistrationDiscoveryTests

open System
open System.Collections.Generic
open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.Domain
open TestsHelpers
open Manisero.AutoRegistrar.TestClasses
open Manisero.AutoRegistrar.TestClasses2
open RegistrationDiscovery

// test data

let intInters = null
let intLifetime = Nullable()
let intReg = new Registration(typeof<int>, InterfaceTypes = intInters, Lifetime = intLifetime)
let r1Inters = new List<Type>()
let r1Lifetime = Nullable()
let r1Reg = new Registration(typeof<R1>, InterfaceTypes = r1Inters, Lifetime = r1Lifetime)
let r2Inters = new List<Type>([typeof<IR2_1>; typeof<IR2_2>])
let r2Lifetime = Nullable 3
let r2Reg = new Registration(typeof<R2>, InterfaceTypes = r2Inters, Lifetime = r2Lifetime)

let testAss = typeof<R1>.Assembly
let initRegs = [intReg; r1Reg; r2Reg]
let typeFilter (typ:Type) = typ.FullName.Contains("1")

// DiscoverRegistrations

[<Fact>]
let ``no initRegs, no filter, single assembly -> concrete classes from assembly`` () =
    let res = DiscoverRegistrations [] None [testAss]

    let types = res |> List.map (fun x -> x.ClassType)
    types |> assertContains [typeof<R1>; typeof<R2>; typeof<C1A_R1>; typeof<NoInters>]
    types |> assertNotContains [typeof<IR2_1>; typeof<R2_Base>; typeof<IR2_Base>]

[<Fact>]
let ``some initRegs -> contains all init types``() =
    let res = DiscoverRegistrations initRegs None [testAss]

    res |> List.map (fun x -> x.ClassType) |> assertContains [intReg.ClassType; r1Reg.ClassType; r2Reg.ClassType]

[<Fact>]
let ``some initRegs -> no duplicates``() =
    let res = DiscoverRegistrations initRegs None [testAss]

    res |> Seq.distinctBy (fun x -> x.ClassType) |> Seq.length |> should equal res.Length

let assertIntersAndLife typ (expInters:IList<Type>) (expLife:Nullable<int>) (regs:Registration list) =
    let reg = regs |> List.find (fun x -> x.ClassType = typ)
    reg.InterfaceTypes |> should equal expInters
    reg.Lifetime |> assertEqualsNullable expLife

[<Fact>]
let ``some initRegs -> interfaceTypes and lifetimes not overriden``() =
    let res = DiscoverRegistrations initRegs None [testAss]

    res |> assertIntersAndLife intReg.ClassType intInters intLifetime
    res |> assertIntersAndLife r1Reg.ClassType r1Inters r1Lifetime
    res |> assertIntersAndLife r2Reg.ClassType r2Inters r2Lifetime

[<Fact>]
let ``some filter -> types filtered``() =
    let res = DiscoverRegistrations [] (Some typeFilter) [testAss]
    
    let types = res |> List.map (fun x -> x.ClassType)
    types |> should contain typeof<R1>
    types |> should not' (contain typeof<R2>)
    types |> should not' (contain typeof<NoInters>)

[<Fact>]
let ``some initRegs, some filter -> filter not applied to init types``() =
    let res = DiscoverRegistrations initRegs (Some typeFilter) [testAss]
    
    let types = res |> List.map (fun x -> x.ClassType)
    types |> assertContains [typeof<R1>; typeof<R2>]
    types |> should not' (contain typeof<NoInters>)

[<Fact>]
let ``multiple assemblies -> types from all assemblies`` () =
    let assemblies = [typeof<TestClass>.Assembly; testAss]

    let res = DiscoverRegistrations [] None assemblies

    let types = res |> List.map (fun x -> x.ClassType)
    types |> assertContains [typeof<TestClass>; typeof<R1>; typeof<R2>]
