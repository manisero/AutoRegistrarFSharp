module AutoRegistrar

open DependencyGraph
open DependancyLevels
open Lifetimes

let FromImplementationMap regs =
    regs |> BuildDependencyGraph |> AssignDependancyLevels |>ResolveLifetimes
