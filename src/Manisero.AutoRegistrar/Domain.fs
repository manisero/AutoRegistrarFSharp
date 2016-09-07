module Domain

open System

type Registration =
    { 
        classType : Type; 
        mutable interfaceTypes : Type list;
        mutable dependencies : Registration list;
        mutable lifetime : int option
    }
