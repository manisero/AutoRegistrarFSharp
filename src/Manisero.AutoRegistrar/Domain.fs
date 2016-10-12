namespace Manisero.AutoRegistrar.Domain

open System

type Registration(classType) =
    static member LongestLifetime = Some 1

    member val ClassType : Type = classType with get, set
    member val InterfaceTypes : Type list option = None with get, set
    member val Dependencies : Registration list = [] with get, set
    member val DependancyLevel : int option = None with get, set
    member val Lifetime : int option = None with get, set

