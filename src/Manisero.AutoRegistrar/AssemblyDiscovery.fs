module AssemblyDiscovery

open System.Collections.Generic
open System.Reflection

let tryAddAss filter (visited:ISet<AssemblyName>) (asses:List<Assembly>) (assName:AssemblyName) =
    match visited.Contains assName with
    | true -> None
    | false ->
        let ass = Assembly.Load assName

        if (Option.isNone filter || (filter.Value ass))
        then asses.Add(ass)

        Some ass

let rec addAssTree filter visited asses assName =
    let ass = tryAddAss filter visited asses assName

    match ass with
    | Some ass -> ass.GetReferencedAssemblies() |> Seq.iter (addAssTree filter visited asses)
    | None -> ignore null

let DiscoverAssemblies (rootAssembly:Assembly) filter =
    let visited = new HashSet<AssemblyName>()
    let asses = new List<Assembly>()
    
    addAssTree filter visited asses (rootAssembly.GetName())

    asses |> Seq.toList
