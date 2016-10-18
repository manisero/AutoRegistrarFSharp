namespace Manisero.AutoRegistrar.Domain

open System
open System.Collections.Generic

[<AllowNullLiteral>]
type Registration(classType) =
    static member LongestLifetime = 1

    member val ClassType : Type = classType with get, set
    member val InterfaceTypes : ICollection<Type> = null with get, set
    member val Dependencies : ICollection<Registration> = new List<Registration>() :> ICollection<Registration> with get, set
    member val DependancyLevel : Nullable<int> = Nullable() with get, set
    member val Lifetime : Nullable<int> = Nullable() with get, set
