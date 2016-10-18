module Manisero.AutoRegistrar.AutoRegistrar

open System
open System.Collections.Generic
open Manisero.AutoRegistrar.Domain
open AssemblyDiscovery
open RegistrationDiscovery
open ImplementationMap
open DependencyGraph
open DependancyLevels
open Lifetimes

let toFun (func:Func<'a, bool>) =
    match func with
    | null -> None
    | f -> Some (fun x -> f.Invoke x)

let FromImplementationMapFs regs = regs |> BuildDependencyGraph |> AssignDependancyLevels |> ResolveLifetimes
let FromImplementationMap regs = FromImplementationMapFs (Seq.toList regs) |> List<Registration>

let FromRegistrationsFs regs = regs |> BuildImplementationMap |> FromImplementationMapFs
let FromRegistrations regs = FromRegistrationsFs (Seq.toList regs) |> List<Registration>

let FromAssembliesFs initRegs typeFilter assemblies = assemblies |> DiscoverRegistrations initRegs typeFilter |> FromRegistrationsFs
let FromAssemblies initRegs typeFilter assemblies = FromAssembliesFs (Seq.toList initRegs) (toFun typeFilter) (Seq.toList assemblies) |> List<Registration>

let FromRootAssemblyFs initRegs rootAssembly assemblyFilter typeFilter = rootAssembly |> DiscoverAssemblies assemblyFilter |> FromAssembliesFs initRegs typeFilter
let FromRootAssembly initRegs rootAssembly assemblyFilter typeFilter = FromRootAssemblyFs (Seq.toList initRegs) rootAssembly (toFun assemblyFilter) (toFun typeFilter) |> List<Registration>
