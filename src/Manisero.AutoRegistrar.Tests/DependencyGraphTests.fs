module DependencyGraphTests

open System
open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open DependencyGraph

// test data

let noDepsReg = { defaultRegistration with classType = typeof<NoDependencies> }
let depOfNoDepsReg =  { defaultRegistration with classType = typeof<DependantOf_NoDependencies> }

// helpers

let assertFails exceptionType action =
    (fun () -> action() |> ignore) |> should throw exceptionType

// BuildDependencyGraph

[<Fact>]
let ``no ctor args -> no deps``() =
    let reg = { noDepsReg with classType = noDepsReg.classType }
    BuildDependencyGraph [reg]

    reg.dependencies |> should equal List.empty<Registration>

// getDependencyTypes

[<Theory>]
[<InlineData(typeof<NoDependencies>, null, null)>]
[<InlineData(typeof<DependantOf_NoDependencies>, typeof<NoDependencies>, null)>]
[<InlineData(typeof<DependantOf_NoDependencies1And2>, typeof<NoDependencies>, typeof<NoDependencies2>)>]
[<InlineData(typeof<DependantOf_NoDependencies_x2>, typeof<NoDependencies>, null)>]
let ``getDepTypes: -> ctor args`` clas (expDep1:Type) (expDep2:Type) =
    let exp = [|expDep1; expDep2|] |> Array.filter (fun x -> x <> null)

    let res = getDepTypes clas

    res |> should equal exp

[<Fact>]
let ``getDepTypes: multiple ctors -> error``() =
    (fun () -> getDepTypes typeof<MultipleConstructors>) |> assertFails typeof<InvalidOperationException>

// findReg

let findRegCases =
    [
        (typeof<NoDependencies>, [noDepsReg], noDepsReg)
    ]

[<Theory>]
[<InlineData(0)>]
let ``findReg: -> matching reg`` case =
    let (typ, regs, exp) = findRegCases.[case]

    let res = findReg typ regs

    res |> should equal exp
