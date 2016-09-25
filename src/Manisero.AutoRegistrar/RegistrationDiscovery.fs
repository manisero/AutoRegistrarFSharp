module RegistrationDiscovery

open System
open System.Collections.Generic
open System.Reflection
open Domain

let buildTypFilter (initTypes:ISet<Type>) =
    (fun (x:Type) -> (not x.IsAbstract) && (not (initTypes.Contains x)))

let getRegsFromAss filter ass =
    let getTypes filter (ass:Assembly) = ass.ExportedTypes |> Seq.filter filter
    let toReg typ = { defaultRegistration with classType = typ }

    ass |> getTypes filter |> Seq.map toReg

let DiscoverRegistrations initRegs filter (assemblies:Assembly list) =
    let initTypes = new HashSet<Type>(initRegs |> Seq.map (fun x -> x.classType))
    let filter = buildTypFilter initTypes

    let newRegs = assemblies |> List.map (getRegsFromAss filter) |> Seq.concat |> Seq.toList
    initRegs @ newRegs
