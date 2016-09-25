module RegistrationDiscoveryTests

open Xunit
open FsUnit.Xunit
open Domain
open Manisero.AutoRegistrar.TestClasses
open RegistrationDiscovery

// DiscoverRegistrations
// no initRegs, no filter, TestClasses assembly -> all concrete classes from TestClasses
// some TestClasses in initRegs -> no duplicates, init lifetimes not overriden
// no initRegs, filter -> filtered
// initRegs, filter -> filter not applied to initRegs
