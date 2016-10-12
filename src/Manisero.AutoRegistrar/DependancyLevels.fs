module Manisero.AutoRegistrar.DependancyLevels

open Manisero.AutoRegistrar.Domain

let tryAssignLvl (reg:Registration) =
    let getMaxDepLvl (reg:Registration) = reg.Dependencies |> List.map (fun x -> x.DependancyLevel.Value) |> List.max

    if (reg.Dependencies |> List.forall (fun x -> x.DependancyLevel.IsSome))
    then
        reg.DependancyLevel <-
            match reg.Dependencies.Length with
            | 0 -> Some 0
            | _ -> Some ((reg |> getMaxDepLvl) + 1)
        
        true
    else
        false

let AssignDependancyLevels regs =
    let hasNoLevel (reg:Registration) = reg.DependancyLevel.IsNone
    let tryAssignAll regs = regs |> List.filter hasNoLevel
                                 |> List.fold (fun anyAssined reg -> (tryAssignLvl reg) || anyAssined) false
    
    let mutable anyAssined = tryAssignAll regs
    while (anyAssined) do anyAssined <- tryAssignAll regs

    if (regs |> List.exists hasNoLevel)
    then
        let failedTypes = regs |> List.filter hasNoLevel |> List.map (fun x -> sprintf "'%s'" x.ClassType.FullName) |> String.concat ", "
        invalidOp (sprintf "Cannot assign dependancy level for the following types: %s. Possible cyclic dependency." failedTypes)

    regs
