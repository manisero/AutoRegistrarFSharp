module Manisero.AutoRegistrar.AutoRegistrar

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
