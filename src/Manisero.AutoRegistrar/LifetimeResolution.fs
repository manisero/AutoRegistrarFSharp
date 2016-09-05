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

let resolveLifetime regToResolve resolvedRegs remainingRegs =
    let resolved = { regToResolve with lifetime = longestLifetime } :: resolvedRegs
    { resolvedRegistrations = resolved; remainingRegistrations = remainingRegs }
