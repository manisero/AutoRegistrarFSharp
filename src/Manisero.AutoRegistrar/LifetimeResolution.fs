module LifetimeResolution

open System

type Registration =
    { 
        classType : Type; 
        interfaceTypes : Type list;
        lifetime : int option
    }

type ResolveLifetimeOutput =
    {
        resolvedRegistrations : Registration list;
        remainingRegistrations : Registration list
    }

let longestLifetime = Some 1
let defaultRegistration = { classType = null; interfaceTypes = []; lifetime = None }

let resolveLifetime startReg otherRegs =
    let resolvedStart = { startReg with lifetime = longestLifetime }
    { resolvedRegistrations = [resolvedStart]; remainingRegistrations = otherRegs }
