module DependencyMap

open System

let getDepTypes (typ:Type) =
    let ctor =
        match typ.GetConstructors() with
        | [| ctor |] -> ctor
        | _ -> invalidOp (sprintf "Cannot identify dependencies of '%s' type as it does not have exactly one constructor." typ.FullName)

    ctor.GetParameters() |> Array.map (fun x -> x.ParameterType)
