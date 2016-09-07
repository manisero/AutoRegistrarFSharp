module LifetimeResolutionTests

open System
open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.TestClasses
open LifetimeResolution

// test data

let r1Reg = { defaultRegistration with classType = typeof<R1> }
let r1Reg_lifetime = { r1Reg with lifetime = Some 3 }

let cR1Reg =  { defaultRegistration with classType = typeof<C_R1> }

let multiCtorsReg =  { defaultRegistration with classType = typeof<MultiCtors> }

// helpers

let assertRegLengths expectedResolvedLength expectedRemainingLength result =
    result.resolvedRegistrations.Length |> should equal expectedResolvedLength
    result.remainingRegistrations.Length |> should equal expectedRemainingLength

let assertReg expectedClassType expectedLifetime reg =
    reg.classType |> should equal expectedClassType
    reg.lifetime |> should equal expectedLifetime

let assertFails exceptionType action =
    (fun () -> action() |> ignore) |> should throw exceptionType

// tests

[<Fact>]
let ``already resolved -> existing registration``() = raise (NotImplementedException())

[<Fact>]
let ``no dependencies -> longestLifetime``() = 
    let result = resolveLifetime r1Reg [] []

    result |> assertRegLengths 1 0
    List.head result.resolvedRegistrations |> assertReg r1Reg.classType longestLifetime

[<Fact>]
let ``single dependency -> derived``() = 
    let result = resolveLifetime cR1Reg [r1Reg_lifetime] []

    result |> assertRegLengths 2 0
    List.head result.resolvedRegistrations |> assertReg cR1Reg.classType r1Reg_lifetime.lifetime

[<Fact>]
let ``multiple constructors -> error``() = 
    (fun () -> resolveLifetime multiCtorsReg [] []) |> assertFails typeof<InvalidOperationException>
