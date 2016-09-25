module ImplementationMap

open System
open System.Collections.Generic
open Domain
open Shared

let validateReg reg =
    if (reg.classType.IsAbstract)
    then invalidOp (sprintf "'%s' cannot be set as Registration.classType as it is abstract." reg.classType.FullName)
    elif (reg.interfaceTypes.IsSome)
    then invalidOp (sprintf "'%s' type's Registration's interfaceTypes is not None. Registration.interfaceTypes must be None." reg.classType.FullName)
    else ignore null

let getClassInterfaces (typ:Type) =
    let baseType = typ.BaseType

    if (baseType = null || baseType = typeof<Object>)
    then Array.toList (typ.GetInterfaces())
    else baseType :: Array.toList (typ.GetInterfaces()) // TODO: Consider walking through full type hierarchy

let handleInterType (handledTypes:ISet<Type>) (typeToRegMap:IDictionary<Type, Registration>) reg inter =
    if (handledTypes.Contains(inter))
    then ignore null
    else
        let mutable existingReg = defaultRegistration

        if (typeToRegMap.TryGetValue(inter, &existingReg))
        then
            existingReg.interfaceTypes <-
                match existingReg.interfaceTypes with
                | Some i -> i |> List.filter (fun x -> x <> inter) |> Some
                | None -> None

            ignore (typeToRegMap.Remove inter)
            ignore (handledTypes.Add inter)
        else
            reg.interfaceTypes <-
                match reg.interfaceTypes with
                | Some i -> Some (inter :: i)
                | None -> Some [inter]

            typeToRegMap.Add(inter, reg)

let BuildImplementationMap regs =
    let handleInters handledTypes typeToRegMap reg = reg.classType |> getClassInterfaces |> List.iter (handleInterType handledTypes typeToRegMap reg)

    regs |> List.iter validateReg

    let handledTypes = buildTypesSet regs
    let typeToRegMap = buildTypeToRegMap regs

    regs |> List.filter (fun x -> x.interfaceTypes.IsNone) |> List.iter (handleInters handledTypes typeToRegMap)
