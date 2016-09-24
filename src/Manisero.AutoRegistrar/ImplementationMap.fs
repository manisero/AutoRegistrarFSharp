module ImplementationMap

open System
open Domain
open Shared

let validateReg reg =
    if (reg.classType.IsAbstract)
    then invalidOp (sprintf "'%s' cannot be set as Registration.classType as it is abstract." reg.classType.FullName)
    elif (reg.interfaceTypes.IsSome)
    then invalidOp (sprintf "'%s' type's Registration's interfaceTypes is not None. Registration.interfaceTypes must be None." reg.classType.FullName)
    else ignore null

let getClassInterfaces (typ:Type) =
    if (typ.BaseType = typeof<Object>)
    then Array.toList (typ.GetInterfaces())
    else typ.BaseType :: Array.toList (typ.GetInterfaces()) // TODO: consider walking through full type hierarchy

let handleInterType handledTypes typeToRegMap reg inter = ignore null
    // - if interface is present in handledTypes, continue
    // - else if interface is present in typeToRegMap.keys
    //   - remove it from its registration interfaceTypes
    //   - remove it from typeToRegMap
    //   - add it to handledTypes
    // - else
    //   - add it to class's interfaceTypes
    //   - add it to typeToRegMap

    // if multiple classes implement given interface, then this interface should not appear in any of those classes' interfaceTypes
    // - e.g. IDisposable can be implemented by multiple classes
    // if a base class of given class is any reg's classType, then the base class should not appear in the class's interfaceTypes

let BuildImplementationMap regs =
    let handleInters handledTypes typeToRegMap reg = reg.classType |> getClassInterfaces |> List.iter (handleInterType handledTypes typeToRegMap reg)

    regs |> List.iter validateReg

    let handledTypes = buildTypesSet regs
    let typeToRegMap = buildTypeToRegMap regs

    regs |> List.filter (fun x -> x.interfaceTypes.IsNone) |> List.iter (handleInters handledTypes typeToRegMap)
