module Manisero.AutoRegistrar.AutoRegistrar

open System
open System.Reflection
open Manisero.AutoRegistrar.Domain
open AssemblyDiscovery
open RegistrationDiscovery
open ImplementationMap
open DependencyGraph
open DependancyLevels
open Lifetimes

let FromImplementationMapFs regs = regs |> BuildDependencyGraph |> AssignDependancyLevels |> ResolveLifetimes |> List.toSeq
let FromImplementationMap regs = FromImplementationMapFs (Seq.toList regs)

let FromRegistrationsFs regs = regs |> BuildImplementationMap |> FromImplementationMapFs
let FromRegistrations regs = FromRegistrationsFs (Seq.toList regs)

let FromAssembliesFs initRegs typeFilter assemblies = assemblies |> DiscoverRegistrations initRegs typeFilter |> FromRegistrationsFs
let FromAssemblies initRegs typeFilter assemblies = FromAssembliesFs (Seq.toList initRegs) typeFilter (Seq.toList assemblies)

let FromRootAssemblyFs initRegs rootAssembly assemblyFilter typeFilter = rootAssembly |> DiscoverAssemblies assemblyFilter |> FromAssembliesFs initRegs typeFilter
let FromRootAssembly initRegs rootAssembly assemblyFilter typeFilter = FromRootAssemblyFs (Seq.toList initRegs) rootAssembly assemblyFilter typeFilter

let FromRootAssemblyCSharp initRegs rootAssembly (assemblyFilter:Converter<Assembly, bool>) (typeFilter:System.Converter<Type, bool>) =
    let assemblyFilterFs = FSharpFunc.FromConverter assemblyFilter
    let typeFilterFs = FSharpFunc.FromConverter typeFilter

    FromRootAssembly initRegs rootAssembly (Some assemblyFilterFs) (Some typeFilterFs)

let FromRootAssemblyCSharp2 (shortLivingType:Type) rootAssembly (assemblyFilter:Converter<Assembly, bool>) (typeFilter:System.Converter<Type, bool>) =
    let initRegs = [new Registration(shortLivingType, Lifetime = Some 3)]
    let assemblyFilterFs = FSharpFunc.FromConverter assemblyFilter
    let typeFilterFs = FSharpFunc.FromConverter typeFilter

    FromRootAssembly initRegs rootAssembly (Some assemblyFilterFs) (Some typeFilterFs)
