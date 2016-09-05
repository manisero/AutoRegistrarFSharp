module LifetimeResolution

open System

type Registration =
    { 
        classType : Type; 
        interfaceTypes : Type list;
        lifetime : int option
    }

type ResolutionResult =
    {
        resolvedRegistrations : Registration list;
        remainingRegistrations : Registration list
    }

let longestLifetime = Some 1
let defaultRegistration = { classType = null; interfaceTypes = []; lifetime = None }

let resolveUnresolved regToResolve resolvedRegs remainingRegs =
    let ctor =
        match regToResolve.classType.GetConstructors() with
        | [| ctor |] -> ctor
        | _ -> invalidOp (sprintf "Cannot identify dependencies of '%s' type as it does not have exactly one constructor." regToResolve.classType.Name)

    //let deps = ctor.GetParameters() |> Array.map (fun x -> x.ParameterType) |> // TODO: call resolveLifetime

    let resolved = { regToResolve with lifetime = longestLifetime } :: resolvedRegs
    { resolvedRegistrations = resolved; remainingRegistrations = remainingRegs }

let resolveLifetime regToResolve resolvedRegs remainingRegs =
    if (resolvedRegs |> List.exists (fun x -> x.classType = regToResolve.classType))
    then { resolvedRegistrations = resolvedRegs; remainingRegistrations = remainingRegs }
    else resolveUnresolved regToResolve resolvedRegs remainingRegs
