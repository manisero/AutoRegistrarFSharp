module Domain

open System

type Registration =
    { 
        classType : Type; 
        mutable interfaceTypes : Type list option;
        mutable dependencies : Registration list;
        mutable dependancyLevel : int option;
        mutable lifetime : int option
    }

let longestLifetime = Some 1
let defaultRegistration = { classType = null; interfaceTypes = None; dependencies = []; dependancyLevel = None; lifetime = None }
