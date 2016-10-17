namespace Manisero.AutoRegistrar.Domain

open System
open System.Collections.Generic

type Registration(classType) =
    static member LongestLifetime = 1

    member val ClassType : Type = classType with get, set
    member val InterfaceTypes : Type list option = None with get, set
    member val Dependencies : ICollection<Registration> = [||] :> ICollection<Registration> with get, set
    member val DependancyLevel : Nullable<int> = Nullable() with get, set
    member val Lifetime : Nullable<int> = Nullable() with get, set
