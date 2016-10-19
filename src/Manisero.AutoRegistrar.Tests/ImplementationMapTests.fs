module Manisero.AutoRegistrar.ImplementationMapTests

open System
open System.Collections.Generic
open Xunit
open FsUnit.Xunit
open Manisero.AutoRegistrar.Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open Internal.ImplementationMap

// test data

let r1Reg = new Registration(typeof<R1>)
let r2Reg = new Registration(typeof<R2>)
let c1aReg = new Registration(typeof<C1A_R1>)
let c1bReg = new Registration(typeof<C1B_R1_R2>)
let c1cReg = new Registration(typeof<C1C_R1_R1>)
let noIntersReg = new Registration(typeof<NoInters>)
let multiImpl1Reg = new Registration(typeof<MultiImpl1>)
let multiImpl2Reg = new Registration(typeof<MultiImpl2>)
let multiImpl2_1Reg = new Registration(typeof<MultiImpl2_1>)
let multiImpl2_2Reg = new Registration(typeof<MultiImpl2_2>)

// getClassInterfaces

let getClassInterfacesCases =
    [
        (typeof<Object>, []);
        (typeof<NoInters>, []);
        (typeof<R1>, [typeof<IR1>]);
        (typeof<R2>, [typeof<R2_Base>; typeof<IR2_Base>; typeof<IR2_1>; typeof<IR2_2>])
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
let ``getClassInterfaces: class -> immediate base class and implemented interfaces`` case =
    let (typ, expInter) = getClassInterfacesCases.[case]

    let res = getClassInterfaces typ

    res |> should equal expInter

// handleInterType

[<Fact>]
let ``handleInterType: inter not in handledTypes nor typeToRegMap keys -> added to reg's interfaceTypes and map``() =
    let r1Reg = new Registration(r1Reg.ClassType)
    let handledTypes = new HashSet<Type>([typeof<R1>])
    let typeToRegMap = new Dictionary<Type, Registration>(dict [(typeof<R1>, r1Reg)])

    handleInterType handledTypes typeToRegMap r1Reg typeof<IR1>

    r1Reg.InterfaceTypes |> should not' (be Null)
    r1Reg.InterfaceTypes |> should contain typeof<IR1>
    typeToRegMap |> should contain (new KeyValuePair<Type, Registration>(typeof<IR1>, r1Reg))
    handledTypes |> should not' (contain typeof<IR1>)
    

[<Fact>]
let ``handleInterType: inter not in handledTypes but in typeToRegMap keys -> removed from reg's interfaceTypes and map and added to handledTypes``() =
    let multiImpl1Reg = new Registration(multiImpl1Reg.ClassType, InterfaceTypes = new List<Type>([typeof<IMultiImpls>]))
    let handledTypes = new HashSet<Type>()
    let typeToRegMap = new Dictionary<Type, Registration>(dict [(typeof<IMultiImpls>, multiImpl1Reg)])

    handleInterType handledTypes typeToRegMap multiImpl1Reg typeof<IMultiImpls>

    multiImpl1Reg.InterfaceTypes |> should not' (contain typeof<IMultiImpls>)
    typeToRegMap |> should not' (contain (new KeyValuePair<Type, Registration>(typeof<IMultiImpls>, multiImpl1Reg)))
    handledTypes |> should contain typeof<IMultiImpls>

[<Fact>]
let ``handleInterType: inter in handledTypes -> ignored``() =
    let r1Reg = new Registration(r1Reg.ClassType, InterfaceTypes = new List<Type>())
    let handledTypes = new HashSet<Type>([typeof<IR1>])

    handleInterType handledTypes null r1Reg typeof<IR1>

    r1Reg.InterfaceTypes |> should not' (contain typeof<IR1>)

// BuildImplementationMap
let BuildImplementationMapCases =
    [
        (r1Reg.ClassType, new List<Type>());
        (r2Reg.ClassType, new List<Type>([typeof<IR2_2>; typeof<IR2_1>; typeof<IR2_Base>; typeof<R2_Base>]));
        (c1aReg.ClassType, new List<Type>([typeof<IC1A_R1>]));
        (noIntersReg.ClassType, new List<Type>());
        (multiImpl1Reg.ClassType, new List<Type>());
        (multiImpl2Reg.ClassType, new List<Type>());
        (multiImpl2_1Reg.ClassType, new List<Type>([typeof<IMultiImpls2_1>]));
        (multiImpl2_2Reg.ClassType, new List<Type>([typeof<IMultiImpls2_2>]))
    ]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(3)>]
[<InlineData(4)>]
[<InlineData(5)>]
[<InlineData(6)>]
[<InlineData(7)>]
let ``BuildImplementationMap: success scenario`` case =
    let regs =
        [
            new Registration(r1Reg.ClassType, InterfaceTypes = new List<Type>());
            new Registration(r2Reg.ClassType);
            new Registration(c1aReg.ClassType);
            new Registration(noIntersReg.ClassType);
            new Registration(multiImpl1Reg.ClassType);
            new Registration(multiImpl2Reg.ClassType);
            new Registration(multiImpl2_1Reg.ClassType, InterfaceTypes = new List<Type>([typeof<IMultiImpls2_1>]));
            new Registration(multiImpl2_2Reg.ClassType, InterfaceTypes = new List<Type>([typeof<IMultiImpls2_2>]))
        ]
    let (regClass, expInters) = BuildImplementationMapCases.[case]

    let res = BuildImplementationMap regs

    res |> should equal regs
    let resInters = regs |> List.find (fun x -> x.ClassType = regClass) |> (fun x -> x.InterfaceTypes)
    resInters |> should not' (be Null)
    resInters |> assertContains expInters

[<Theory>]
[<InlineData(typeof<IR2_1>)>]
[<InlineData(typeof<R2_Base>)>]
let ``BuildImplementationMap: reg classType is abstract or interface -> error`` classType =
    let reg = new Registration(classType)
    
    (fun () -> BuildImplementationMap [reg]) |> assertInvalidOp
