module Manisero.AutoRegistrar.Shared

open System
open System.Collections.Generic
open Domain

let buildTypesSet (regs:Registration list) =
    let addToSet (set:HashSet<Type>) typ =
        match set.Add typ with
        | true -> ignore null
        | false -> invalidOp (sprintf "Multiple registrations found for '%s' type." typ.FullName)

    let set = new HashSet<Type>()
    regs |> List.map (fun x -> x.classType :: (defaultArg x.interfaceTypes [])) |> List.concat |> List.iter (addToSet set)

    set :> ISet<Type>

let buildTypeToRegMap (regs:Registration list) =
    let getInterToRegList reg = (defaultArg reg.interfaceTypes []) |> List.map (fun x -> (x, reg))

    let addToMap (map:Dictionary<Type, Registration>) (typ, reg) =
        if (map.ContainsKey typ)
        then invalidOp (sprintf "Multiple registrations found for '%s' type." typ.FullName)
        else map.Add(typ, reg)
    
    let map = new Dictionary<Type, Registration>()

    regs |> List.map (fun x -> (x.classType, x) :: getInterToRegList x) |> List.concat |> List.iter (addToMap map)
    map :> IDictionary<Type, Registration>
