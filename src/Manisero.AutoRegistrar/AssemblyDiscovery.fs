module AssemblyDiscovery

open System
open System.Collections.Generic
open System.Reflection

let getAssNameComparer() =
    {
        new IEqualityComparer<AssemblyName> with
            member this.Equals(x, y) = x.FullName.Equals(y.FullName, StringComparison.Ordinal)
            member this.GetHashCode(obj) = obj.FullName.GetHashCode()
    }

let tryAddAss filter (visited:ISet<AssemblyName>) (asses:List<Assembly>) (assName:AssemblyName) =
    match visited.Contains assName with
    | true -> None
    | false ->
        let ass = Assembly.Load assName

        if (Option.isNone filter || (filter.Value ass))
        then asses.Add(ass)

        ignore (visited.Add assName)
        Some ass

let rec addAssTree filter visited asses assName =
    let ass = tryAddAss filter visited asses assName

    match ass with
    | Some ass -> ass.GetReferencedAssemblies() |> Seq.iter (addAssTree filter visited asses)
    | None -> ignore null

let DiscoverAssemblies assemblyFilter (rootAssembly:Assembly) =
    let visited = new HashSet<AssemblyName>(getAssNameComparer())
    let asses = new List<Assembly>()
    
    addAssTree assemblyFilter visited asses (rootAssembly.GetName())

    asses |> Seq.toList
