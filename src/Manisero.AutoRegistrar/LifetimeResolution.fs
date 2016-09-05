module LifetimeResolution

open System

type Registration =
    { 
        classType : Type; 
        interfaceType : Type list;
        lifetime : int
    }

let resolveLifetimes regs = null

let resolveLifetime classType regs = null
