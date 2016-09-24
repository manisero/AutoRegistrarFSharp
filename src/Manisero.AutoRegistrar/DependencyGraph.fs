module DependencyGraph

open System
open System.Collections.Generic
open Shared
open Domain

let getDepTypes (clas:Type) =
    let ctor =
        match clas.GetConstructors() with
        | [| ctor |] -> ctor
        | _ -> invalidOp (sprintf "Cannot identify dependencies of '%s' type as it does not have exactly one constructor." clas.FullName)

    ctor.GetParameters() |> Array.map (fun x -> x.ParameterType) |> Array.distinct |> Array.toList

let findReg (map:IDictionary<Type, Registration>) typ =
    let mutable reg = defaultRegistration

    match map.TryGetValue(typ, &reg) with
    | true -> reg
    | false -> invalidOp (sprintf "Cannot find matching registration for '%s' type." typ.FullName)

let BuildDependencyGraph regs =
    let map = buildTypeToRegMap regs
    let getDeps reg = reg.classType |> getDepTypes |> List.map (findReg map)

    regs |> List.iter (fun x -> x.dependencies <- getDeps x)
