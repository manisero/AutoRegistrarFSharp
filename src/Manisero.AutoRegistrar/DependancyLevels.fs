module DependancyLevels

open Domain

let tryAssignLvl reg =
    let getMaxDepLvl reg = reg.dependencies |> List.map (fun x -> x.dependancyLevel.Value) |> List.max

    if (reg.dependencies |> List.forall (fun x -> x.dependancyLevel.IsSome))
    then
        reg.dependancyLevel <-
            match reg.dependencies.Length with
            | 0 -> Some 0
            | _ -> Some ((reg |> getMaxDepLvl) + 1)
        
        true
    else
        false

let AssignDependancyLevels regs =
    let hasNoLevel reg = reg.dependancyLevel.IsNone
    let tryAssignAll regs = regs |> List.filter hasNoLevel
                                 |> List.fold (fun anyAssined reg -> (tryAssignLvl reg) || anyAssined) false
    
    let mutable anyAssined = tryAssignAll regs
    while (anyAssined) do anyAssined <- tryAssignAll regs

    if (regs |> List.exists hasNoLevel)
    then
        let failedTypes = regs |> List.filter hasNoLevel |> List.map (fun x -> sprintf "'%s'" x.classType.FullName) |> String.concat ", "
        invalidOp (sprintf "Cannot assign dependancy level for the following types: %s." failedTypes)

    regs
