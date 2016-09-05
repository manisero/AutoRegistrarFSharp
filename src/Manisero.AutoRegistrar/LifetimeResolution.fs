﻿module LifetimeResolution

open System

type Registration =
    { 
        classType : Type; 
        interfaceTypes : Type list;
        lifetime : int option
    }

let longestLifetime = 1
let defaultRegistration = { classType = null; interfaceTypes = []; lifetime = None }

let resolveLifetime reg otherRegs = reg
