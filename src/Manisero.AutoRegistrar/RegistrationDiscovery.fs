module RegistrationDiscovery

open System.Reflection
open Domain

let DiscoverRegistrations initRegs filter (assemblies:Assembly list) =
    let getTypes (ass:Assembly) = ass.ExportedTypes |> Seq.filter (fun x -> not x.IsAbstract)
    let toReg typ = { defaultRegistration with classType = typ }

    assemblies |> List.map (fun x -> x |> getTypes |> Seq.map toReg ) |> Seq.concat |> List.ofSeq
