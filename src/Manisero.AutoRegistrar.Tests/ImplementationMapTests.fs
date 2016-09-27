module ImplementationMapTests

open System
open System.Collections.Generic
open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open TestsHelpers
open ImplementationMap

// test data

let r1Reg = { defaultRegistration with classType = typeof<R1>; }
let r2Reg = { defaultRegistration with classType = typeof<R2>; }
let c1aReg = { defaultRegistration with classType = typeof<C1A_R1>;}
let c1bReg = { defaultRegistration with classType = typeof<C1B_R1_R2>; }
let c1cReg = { defaultRegistration with classType = typeof<C1C_R1_R1>; }
let noIntersReg = { defaultRegistration with classType = typeof<NoInters>; }
let multiImpl1Reg = { defaultRegistration with classType = typeof<MultiImpl1>; }
let multiImpl2Reg = { defaultRegistration with classType = typeof<MultiImpl2>; }
let multiImpl2_1Reg = { defaultRegistration with classType = typeof<MultiImpl2_1>; }
let multiImpl2_2Reg = { defaultRegistration with classType = typeof<MultiImpl2_2>; }

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
    let r1Reg = { r1Reg with classType = r1Reg.classType }
    let handledTypes = new HashSet<Type>([typeof<R1>])
    let typeToRegMap = new Dictionary<Type, Registration>(dict [(typeof<R1>, r1Reg)])

    handleInterType handledTypes typeToRegMap r1Reg typeof<IR1>

    r1Reg.interfaceTypes |> should not' (be Null)
    r1Reg.interfaceTypes.Value |> should contain typeof<IR1>
    typeToRegMap |> should contain (new KeyValuePair<Type, Registration>(typeof<IR1>, r1Reg))
    handledTypes |> should not' (contain typeof<IR1>)
    

[<Fact>]
let ``handleInterType: inter not in handledTypes but in typeToRegMap keys -> removed from reg's interfaceTypes and map and added to handledTypes``() =
    let multiImpl1Reg = { multiImpl1Reg with interfaceTypes = Some [typeof<IMultiImpls>] }
    let handledTypes = new HashSet<Type>()
    let typeToRegMap = new Dictionary<Type, Registration>(dict [(typeof<IMultiImpls>, multiImpl1Reg)])

    handleInterType handledTypes typeToRegMap multiImpl1Reg typeof<IMultiImpls>

    multiImpl1Reg.interfaceTypes.Value |> should not' (contain typeof<IMultiImpls>)
    typeToRegMap |> should not' (contain (new KeyValuePair<Type, Registration>(typeof<IMultiImpls>, multiImpl1Reg)))
    handledTypes |> should contain typeof<IMultiImpls>

[<Fact>]
let ``handleInterType: inter in handledTypes -> ignored``() =
    let r1Reg = { r1Reg with interfaceTypes = Some [] }
    let handledTypes = new HashSet<Type>([typeof<IR1>])

    handleInterType handledTypes null r1Reg typeof<IR1>

    r1Reg.interfaceTypes.Value |> should not' (contain typeof<IR1>)

// BuildImplementationMap
let BuildImplementationMapCases =
    [
        (r1Reg.classType, []);
        (r2Reg.classType, [typeof<IR2_2>; typeof<IR2_1>; typeof<IR2_Base>; typeof<R2_Base>]);
        (c1aReg.classType, [typeof<IC1A_R1>]);
        (noIntersReg.classType, []);
        (multiImpl1Reg.classType, []);
        (multiImpl2Reg.classType, []);
        (multiImpl2_1Reg.classType, [typeof<IMultiImpls2_1>]);
        (multiImpl2_2Reg.classType, [typeof<IMultiImpls2_2>])
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
            { r1Reg with interfaceTypes = Some [] };
            { r2Reg with classType = r2Reg.classType };
            { c1aReg with classType = c1aReg.classType };
            { noIntersReg with classType = noIntersReg.classType };
            { multiImpl1Reg with classType = multiImpl1Reg.classType };
            { multiImpl2Reg with classType = multiImpl2Reg.classType };
            { multiImpl2_1Reg with interfaceTypes = Some [typeof<IMultiImpls2_1>] };
            { multiImpl2_2Reg with interfaceTypes = Some [typeof<IMultiImpls2_2>] }
        ]
    let (regClass, expInters) = BuildImplementationMapCases.[case]

    let res = BuildImplementationMap regs

    res |> should equal regs
    let resInters = regs |> List.find (fun x -> x.classType = regClass) |> (fun x -> x.interfaceTypes)
    resInters |> should not' (be Null)
    resInters.Value |> should equal expInters

[<Theory>]
[<InlineData(typeof<IR2_1>)>]
[<InlineData(typeof<R2_Base>)>]
let ``BuildImplementationMap: reg classType is abstract or interface -> error`` classType =
    let reg = { defaultRegistration with classType = classType }
    
    (fun () -> BuildImplementationMap [reg]) |> assertInvalidOp
