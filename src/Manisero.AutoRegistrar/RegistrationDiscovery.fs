module Manisero.AutoRegistrar.RegistrationDiscovery

open System
open System.Collections.Generic
open System.Reflection
open Manisero.AutoRegistrar.Domain

let buildTypFilter (initTypes:ISet<Type>) customFilter =
    match customFilter with
    | Some f -> fun (x:Type) -> (not x.IsAbstract) && (not (initTypes.Contains x)) && f x
    | _ -> fun (x:Type) -> (not x.IsAbstract) && (not (initTypes.Contains x))

let getRegsFromAss filter ass =
    let getTypes filter (ass:Assembly) = ass.ExportedTypes |> Seq.filter filter
    let toReg typ = new Registration(typ)

    ass |> getTypes filter |> Seq.map toReg

let DiscoverRegistrations initRegs typeFilter (assemblies:Assembly list) =
    let initTypes = new HashSet<Type>(initRegs |> List.map (fun (x:Registration) -> x.ClassType))
    let filter = buildTypFilter initTypes typeFilter

    let newRegs = assemblies |> List.map (getRegsFromAss filter) |> Seq.concat |> Seq.toList
    initRegs @ newRegs
