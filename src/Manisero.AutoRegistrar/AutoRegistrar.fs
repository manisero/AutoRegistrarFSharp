module Manisero.AutoRegistrar.AutoRegistrar

open System
open System.Collections.Generic
open System.Linq
open System.Reflection
open Domain
open AssemblyDiscovery
open RegistrationDiscovery
open ImplementationMap
open DependencyGraph
open DependancyLevels
open Lifetimes

let FromImplementationMap regs = regs |> BuildDependencyGraph |> AssignDependancyLevels |> ResolveLifetimes

let FromRegistrations regs = regs |> BuildImplementationMap |> FromImplementationMap

let FromAssemblies initRegs typeFilter assemblies = assemblies |> DiscoverRegistrations initRegs typeFilter |> FromRegistrations

let FromRootAssembly initRegs rootAssembly assemblyFilter typeFilter = rootAssembly |> DiscoverAssemblies assemblyFilter |> FromAssemblies initRegs typeFilter

let FromRootAssemblyCSharp (initRegs:IList<Registration>) rootAssembly (assemblyFilter:Converter<Assembly, bool>) (typeFilter:System.Converter<Type, bool>) =
    let initRegsFSharp = Seq.toList initRegs
    let assemblyFilterFSharp = FSharpFunc.FromConverter assemblyFilter
    let typeFilterFSharp = FSharpFunc.FromConverter typeFilter

    let regs = rootAssembly |> DiscoverAssemblies (Some assemblyFilterFSharp) |> FromAssemblies initRegsFSharp (Some typeFilterFSharp)
    regs.ToList()
