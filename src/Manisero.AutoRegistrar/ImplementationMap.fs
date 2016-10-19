module Manisero.AutoRegistrar.Internal.ImplementationMap

open System
open System.Collections.Generic
open Manisero.AutoRegistrar.Domain
open Shared

let validateReg (reg:Registration) =
    if (reg.ClassType.IsAbstract)
    then invalidOp (sprintf "'%s' cannot be set as Registration.classType as it is abstract." reg.ClassType.FullName)
    else ignore null

let getClassInterfaces (typ:Type) =
    let baseType = typ.BaseType

    if (baseType = null || baseType = typeof<Object>)
    then Array.toList (typ.GetInterfaces())
    else baseType :: Array.toList (typ.GetInterfaces()) // TODO: Consider walking through full type hierarchy

let handleInterType (handledTypes:ISet<Type>) (typeToRegMap:IDictionary<Type, Registration>) (reg:Registration) inter =
    let handleConflict (existingReg:Registration) =
        match existingReg.InterfaceTypes with
        | null -> ignore null
        | inters -> ignore (inters.Remove inter)

        ignore (typeToRegMap.Remove inter)
        ignore (handledTypes.Add inter)

    let handleNew() =
        match reg.InterfaceTypes with
        | null -> reg.InterfaceTypes <- new List<Type>([inter])
        | inters -> inters.Add(inter)

        typeToRegMap.Add(inter, reg)

    if (handledTypes.Contains inter)
    then ignore null
    else
        let mutable existingReg = null

        if (typeToRegMap.TryGetValue(inter, &existingReg))
        then handleConflict existingReg
        else handleNew()

let BuildImplementationMap regs =
    let handleInters handledTypes typeToRegMap (reg:Registration) =
        reg.ClassType |> getClassInterfaces |> List.iter (handleInterType handledTypes typeToRegMap reg)

        if (isNull reg.InterfaceTypes)
        then reg.InterfaceTypes <- new List<Type>()

    regs |> List.iter validateReg

    let handledTypes = buildTypesSet regs
    let typeToRegMap = buildTypeToRegMap regs

    regs |> List.filter (fun x -> isNull x.InterfaceTypes) |> List.iter (handleInters handledTypes typeToRegMap)
    regs
