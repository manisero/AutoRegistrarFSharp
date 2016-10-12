module Manisero.AutoRegistrar.Lifetimes

open Manisero.AutoRegistrar.Domain

let resolveLifetime (reg:Registration) =
    if (reg.Lifetime.IsSome)
        then ignore null
    elif (reg.Dependencies.Length = 0)
        then reg.Lifetime <- Registration.LongestLifetime
    elif (reg.Dependencies |> List.forall (fun x -> x.Lifetime.IsSome))
        then reg.Lifetime <- Some (reg.Dependencies |> List.map (fun x -> x.Lifetime.Value) |> List.max)
    else
        invalidOp (sprintf "Cannot resolve lifetime for '%s' type as some of its dependencies do not have lifetime assigned." reg.ClassType.FullName)

let ResolveLifetimes regs =
    let checkReg (reg:Registration) =
        if (reg.DependancyLevel.IsNone)
        then invalidOp (sprintf "Cannot resolve lifetime for '%s' type as it does not have dependancyLevel assigned." reg.ClassType.FullName)

    regs |> List.iter checkReg
    regs |> List.sortBy (fun x -> x.DependancyLevel.Value) |> List.iter resolveLifetime
    regs
