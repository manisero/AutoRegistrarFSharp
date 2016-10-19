module Manisero.AutoRegistrar.Internal.Shared

open System
open System.Collections.Generic
open Manisero.AutoRegistrar

let buildTypesSet (regs:Registration list) =
    let addTypToSet (set:HashSet<Type>) typ =
        match set.Add typ with
        | true -> ignore null
        | false -> invalidOp (sprintf "Multiple registrations found for '%s' type." typ.FullName)

    let addRegToSet (set:HashSet<Type>) (reg:Registration) =
        addTypToSet set reg.ClassType

        if (not (isNull reg.InterfaceTypes))
        then reg.InterfaceTypes |> Seq.iter (addTypToSet set)

    let set = new HashSet<Type>()
    regs |> Seq.iter (addRegToSet set)

    set :> ISet<Type>

let buildTypeToRegMap (regs:Registration list) =
    let addTypToMap (map:Dictionary<Type, Registration>) reg typ =
        if (map.ContainsKey typ)
        then invalidOp (sprintf "Multiple registrations found for '%s' type." typ.FullName)
        else map.Add(typ, reg)

    let addRegToMap (map:Dictionary<Type, Registration>) reg =
        addTypToMap map reg reg.ClassType

        if (not (isNull reg.InterfaceTypes))
        then reg.InterfaceTypes |> Seq.iter (addTypToMap map reg)
    
    let map = new Dictionary<Type, Registration>()
    regs |> Seq.iter (addRegToMap map)

    map :> IDictionary<Type, Registration>
