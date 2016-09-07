module DependencyGraph

open System

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

let findReg typ regs = ignore null
