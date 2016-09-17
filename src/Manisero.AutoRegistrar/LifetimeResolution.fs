module LifetimeResolution

open Domain

let resolveLifetime reg =
    if (reg.lifetime.IsSome)
        then ignore null
    elif (reg.dependencies.Length = 0)
        then reg.lifetime <- longestLifetime
    elif (reg.dependencies |> List.forall (fun x -> x.lifetime.IsSome))
        then reg.lifetime <- Some (reg.dependencies |> List.map (fun x -> x.lifetime.Value) |> List.max)
    else
        invalidOp (sprintf "Cannot resolve lifetime for '%s' type as some of its dependencies do not have lifetime assigned." reg.classType.FullName)

let resolveLifetimes (resolveLifetime:Registration -> unit) regs =
    let checkReg reg =
        if (reg.dependancyLevel.IsNone)
        then invalidOp (sprintf "Cannot resolve lifetime for '%s' type as it does not have dependancyLevel assigned." reg.classType.FullName)

    regs |> List.iter checkReg
    regs |> List.sortBy (fun x -> x.dependancyLevel.Value) |> List.iter resolveLifetime

let ResolveLifetimes = resolveLifetimes resolveLifetime
