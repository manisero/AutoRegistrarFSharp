module DependencyGraph

open System
open System.Collections.Generic
open Domain

let buildTypeToRegMap (regs:Registration list) = // TODO: assume that classTypes and interfaceTypes are unique
    let getInterToRegList reg = reg.interfaceTypes |> List.map (fun x -> (x, reg))

    let addToMap (map:Dictionary<Type, Registration>) (typ, reg) =
        if (map.ContainsKey typ)
        then invalidOp (sprintf "Multiple registrations found for '%s' type." typ.FullName)
        else map.Add(typ, reg)
    
    let map = new Dictionary<Type, Registration>()

    regs |> List.map (fun x -> (x.classType, x) :: getInterToRegList x) |> List.concat |> List.iter (addToMap map)
    map

let getDepTypes (clas:Type) =
    let ctor =
        match clas.GetConstructors() with
        | [| ctor |] -> ctor
        | _ -> invalidOp (sprintf "Cannot identify dependencies of '%s' type as it does not have exactly one constructor." clas.FullName)

    ctor.GetParameters() |> Array.map (fun x -> x.ParameterType) |> Array.distinct |> Array.toList

let findReg (regs:Registration list) (typ:Type) =
    let byClassType() = regs |> List.tryFind (fun x -> x.classType = typ)
    let byInterfaces() = regs |> List.tryFind (fun x -> x.interfaceTypes |> List.contains typ)

    let reg = if not typ.IsAbstract
              then byClassType()
              else match byClassType() with
                    | None -> byInterfaces()
                    | some -> some

    match reg with
    | Some reg -> reg
    | None -> invalidOp (sprintf "Cannot find matching registration for '%s' type." typ.FullName)

let buildDependencyGraph (getDepTypes:Type -> Type list) (findReg:Registration list -> Type -> Registration) (regs:Registration list) =
    let getDeps reg = reg.classType |> getDepTypes |> List.map (findReg regs)

    regs |> List.iter (fun x -> x.dependencies <- getDeps x)

let BuildDependencyGraph = buildDependencyGraph getDepTypes findReg
