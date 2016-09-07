module DependencyGraph

open System
open Domain

let BuildDependencyGraph regs =
    ignore null
    // for each reg
    // - getDependencyTypes
    // - for each type
    //   - findReg
    // reg.deps <- regs

let getDepTypes (clas:Type) =
    let ctor =
        match clas.GetConstructors() with
        | [| ctor |] -> ctor
        | _ -> invalidOp (sprintf "Cannot identify dependencies of '%s' type as it does not have exactly one constructor." clas.Name)

    ctor.GetParameters() |> Array.map (fun x -> x.ParameterType) |> Array.distinct

let findReg (typ:Type) (regs:Registration list) =
    let byClassType() = regs |> List.tryFind (fun x -> x.classType = typ)

    let reg = if not typ.IsAbstract
              then byClassType()
              else byClassType() // TODO: by interfaces

    match reg with
    | Some reg -> reg
    | None -> invalidOp (sprintf "Cannot find matching registration for '%s' type." typ.Name)
