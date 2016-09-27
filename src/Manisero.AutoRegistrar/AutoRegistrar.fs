module AutoRegistrar

open AssemblyDiscovery
open RegistrationDiscovery
open ImplementationMap
open DependencyGraph
open DependancyLevels
open Lifetimes

let FromImplementationMap = BuildDependencyGraph >> AssignDependancyLevels >>ResolveLifetimes

let FromRegistrations = BuildImplementationMap >> FromImplementationMap

let FromAssemblies initRegs typeFilter assemblies = assemblies |> DiscoverRegistrations initRegs typeFilter |> FromRegistrations

let FromRootAssembly initRegs rootAssembly assemblyFilter typeFilter = rootAssembly |> DiscoverAssemblies assemblyFilter |> FromAssemblies initRegs typeFilter
