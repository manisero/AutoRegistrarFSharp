module Manisero.AutoRegistrar.Registrar

open System
open System.Collections.Generic
open Internal.AssemblyDiscovery
open Internal.RegistrationDiscovery
open Internal.ImplementationMap
open Internal.DependencyGraph
open Internal.DependancyLevels
open Internal.Lifetimes

let private toFun (func:Func<'a, bool>) =
    match func with
    | null -> None
    | f -> Some (fun x -> f.Invoke x)

let FromImplementationMapFs registrations = registrations |> BuildDependencyGraph |> AssignDependancyLevels |> ResolveLifetimes
let FromImplementationMap registrations = FromImplementationMapFs (Seq.toList registrations) |> List<Registration>

let FromRegistrationsFs registrations = registrations |> BuildImplementationMap |> FromImplementationMapFs
let FromRegistrations registrations = FromRegistrationsFs (Seq.toList registrations) |> List<Registration>

let FromAssembliesFs initialRegistrations typeFilter assemblies = assemblies |> DiscoverRegistrations initialRegistrations typeFilter |> FromRegistrationsFs
let FromAssemblies initialRegistrations typeFilter assemblies = FromAssembliesFs (Seq.toList initialRegistrations) (toFun typeFilter) (Seq.toList assemblies) |> List<Registration>

let FromRootAssemblyFs initialRegistrations rootAssembly assemblyFilter typeFilter = rootAssembly |> DiscoverAssemblies assemblyFilter |> FromAssembliesFs initialRegistrations typeFilter
let FromRootAssembly initialRegistrations rootAssembly assemblyFilter typeFilter = FromRootAssemblyFs (Seq.toList initialRegistrations) rootAssembly (toFun assemblyFilter) (toFun typeFilter) |> List<Registration>
