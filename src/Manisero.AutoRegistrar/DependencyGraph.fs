module DependencyGraph

open System
open Domain

let getDepTypes (clas:Type) =
    let ctor =
        match clas.GetConstructors() with
        | [| ctor |] -> ctor
        | _ -> invalidOp (sprintf "Cannot identify dependencies of '%s' type as it does not have exactly one constructor." clas.Name)

    ctor.GetParameters() |> Array.map (fun x -> x.ParameterType) |> Array.distinct

let findReg (typ:Type) (regs:Registration list) =
    let byClassType() = regs |> List.tryFind (fun x -> x.classType = typ)
    let byInterfaces() = regs |> List.tryFind (fun x -> x.interfaceTypes |> List.contains typ)

    let reg = if not typ.IsAbstract
              then byClassType()
              else match byClassType() with
                    | None -> byInterfaces()
                    | some -> some

    match reg with
    | Some reg -> reg
    | None -> invalidOp (sprintf "Cannot find matching registration for '%s' type." typ.Name)

let buildDependencyGraph (getDepTypes:Type -> Type[]) findReg (regs:Registration list) =
    let depTypes = getDepTypes regs.[0].classType
    let reg = findReg depTypes.[0] regs
    ignore null
    // for each reg
    // - getDepTypes
    // - for each type
    //   - findReg
    // - reg.deps <- regs

let BuildDependencyGraph = buildDependencyGraph getDepTypes findReg
