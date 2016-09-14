﻿module DependancyLevels

open Domain

let tryAssignLvl reg =
    let getMaxDepLvl reg = reg.dependencies |> List.map (fun x -> x.dependancyLevel) |> List.max

    if (reg.dependencies |> List.forall (fun x -> x.dependancyLevel.IsSome))
    then
        reg.dependancyLevel <-
            match reg.dependencies.Length with
            | 0 -> Some 0
            | _ -> Some ((reg |> getMaxDepLvl |> Option.get) + 1)
        
        true
    else
        false

let assignDependancyLevels (tryAssignLvl:Registration -> bool) (regs:Registration list) =
    ignore null
    // repeat until all tryAssignLvl invocations return false:
    // - for each reg where lvl = null, tryAssignLvl
    // check if all regs have lvl
    // - if not -> exception

let AssignDependancyLevels = assignDependancyLevels tryAssignLvl
