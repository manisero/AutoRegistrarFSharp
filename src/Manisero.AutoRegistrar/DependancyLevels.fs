module DependancyLevels

open Domain

let tryAssignLvl reg =
    false
    // no dependencies -> assign 0; true
    // dependencies
    //  - all dependencies have lvl -> assign highest dependency level + 1; true
    //  - else -> false

let assignDependancyLevels (tryAssignLvl:Registration -> bool) (regs:Registration list) =
    ignore null
    // repeat until all tryAssignLvl invocations return false:
    // - for each reg where lvl = null, tryAssignLvl
    // check if all regs have lvl
    // - if not -> exception

let AssignDependancyLevels = assignDependancyLevels tryAssignLvl
