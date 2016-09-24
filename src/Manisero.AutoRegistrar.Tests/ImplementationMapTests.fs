module ImplementationMapTests

open System.Linq
open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open ImplementationMap


// getClassInterfaces

let getClassInterfacesCases =
    [
        (typeof<NoInters>, [||]);
        (typeof<R1>, [|typeof<IR1>|]);
        (typeof<R2>, [|typeof<R2_Base>; typeof<IR2_Base>; typeof<IR2_1>; typeof<IR2_2>|])
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
let ``getClassInterfaces: class -> immediate base class and implemented interfaces`` case =
    let (typ, expInter) = getClassInterfacesCases.[case]

    let res = getClassInterfaces typ

    res.ToArray() |> should equal expInter

// handleInterType

[<Fact>]
let ``handleInterType: inter not in handledTypes nor typeToRegMap keys -> added to reg's interfaceTypes and map``() =
    failwith "TODO"
    ignore null

[<Fact>]
let ``handleInterType: inter in handledTypes -> ignored``() =
    failwith "TODO"
    ignore null

[<Fact>]
let ``handleInterType: inter in typeToRegMap keys -> removed from reg's interfaceTypes and map and added to handledTypes``() =
    failwith "TODO"
    ignore null

// BuildImplementationMap

[<Fact>]
let ``BuildImplementationMap: success scenario``() =
    failwith "TODO"
    ignore null

[<Fact>]
let ``BuildImplementationMap: reg classType is abstract or interface -> error``() =
    failwith "TODO"
    ignore null

[<Fact>]
let ``BuildImplementationMap: reg interfaceType is not None -> error``() =
    failwith "TODO"
    ignore null
