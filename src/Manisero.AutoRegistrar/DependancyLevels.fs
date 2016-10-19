module Manisero.AutoRegistrar.Internal.DependancyLevels

open System
open Manisero.AutoRegistrar

let tryAssignLvl (reg:Registration) =
    let getMaxDepLvl (reg:Registration) = reg.Dependencies |> Seq.map (fun x -> x.DependancyLevel.Value) |> Seq.max

    if (reg.Dependencies |> Seq.forall (fun x -> x.DependancyLevel.HasValue))
    then
        reg.DependancyLevel <-
            match reg.Dependencies.Count with
            | 0 -> Nullable 0
            | _ -> Nullable ((reg |> getMaxDepLvl) + 1)
        
        true
    else
        false

let AssignDependancyLevels regs =
    let hasNoLevel (reg:Registration) = not reg.DependancyLevel.HasValue
    let tryAssignAll regs = regs |> List.filter hasNoLevel
                                 |> List.fold (fun anyAssined reg -> (tryAssignLvl reg) || anyAssined) false
    
    let mutable anyAssined = tryAssignAll regs
    while (anyAssined) do anyAssined <- tryAssignAll regs

    if (regs |> List.exists hasNoLevel)
    then
        let failedTypes = regs |> List.filter hasNoLevel |> List.map (fun x -> sprintf "'%s'" x.ClassType.FullName) |> String.concat ", "
        invalidOp (sprintf "Cannot assign dependancy level for the following types: %s. Possible cyclic dependency." failedTypes)

    regs
