module Shared

open System
open System.Collections.Generic
open Domain

let buildTypeToRegMap (regs:Registration list) =
    let getInterToRegList reg = reg.interfaceTypes |> List.map (fun x -> (x, reg))

    let addToMap (map:Dictionary<Type, Registration>) (typ, reg) =
        if (map.ContainsKey typ)
        then invalidOp (sprintf "Multiple registrations found for '%s' type." typ.FullName)
        else map.Add(typ, reg)
    
    let map = new Dictionary<Type, Registration>()

    regs |> List.map (fun x -> (x.classType, x) :: getInterToRegList x) |> List.concat |> List.iter (addToMap map)
    map :> IDictionary<Type, Registration>
