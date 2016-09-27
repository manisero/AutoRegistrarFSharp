module ImplementationMap

open System
open System.Collections.Generic
open Domain
open Shared

let validateReg reg =
    if (reg.classType.IsAbstract)
    then invalidOp (sprintf "'%s' cannot be set as Registration.classType as it is abstract." reg.classType.FullName)
    else ignore null

let getClassInterfaces (typ:Type) =
    let baseType = typ.BaseType

    if (baseType = null || baseType = typeof<Object>)
    then Array.toList (typ.GetInterfaces())
    else baseType :: Array.toList (typ.GetInterfaces()) // TODO: Consider walking through full type hierarchy

let handleInterType (handledTypes:ISet<Type>) (typeToRegMap:IDictionary<Type, Registration>) reg inter =
    let handleConflict existingReg =
        existingReg.interfaceTypes <- (defaultArg existingReg.interfaceTypes []) |> List.filter (fun x -> x <> inter) |> Some
        ignore (typeToRegMap.Remove inter)
        ignore (handledTypes.Add inter)

    let handleNew() =
        reg.interfaceTypes <- inter :: (defaultArg reg.interfaceTypes []) |> Some
        typeToRegMap.Add(inter, reg)

    if (handledTypes.Contains(inter))
    then ignore null
    else
        let mutable existingReg = defaultRegistration

        if (typeToRegMap.TryGetValue(inter, &existingReg))
        then handleConflict existingReg
        else handleNew()

let BuildImplementationMap regs =
    let handleInters handledTypes typeToRegMap reg =
        reg.classType |> getClassInterfaces |> List.iter (handleInterType handledTypes typeToRegMap reg)

        if (reg.interfaceTypes.IsNone)
        then reg.interfaceTypes <- Some []

    regs |> List.iter validateReg

    let handledTypes = buildTypesSet regs
    let typeToRegMap = buildTypeToRegMap regs

    regs |> List.filter (fun x -> x.interfaceTypes.IsNone) |> List.iter (handleInters handledTypes typeToRegMap)
    regs
