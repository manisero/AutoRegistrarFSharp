module ImplementationMap

let buildImplementationMap regs =
    // regs contains all registrations to fill (fill = set interfaceTypes)
    // each reg can contain lifetime, but should not contain interfaceTypes
    
    // for each reg:
    // - it's interfaces are interfaces and class it implements
    //   - TODO: consider walking through full type hierarchy
    // if multiple classes implement given interface, then this interface should not appear in any those classes' interfaceTypes
    // - e.g. IDisposable can be implemented by multiple classes
    // if a base class of given class is any reg's classType, then the base class should not appear in the class's interfaceTypes

    // OBSOLETE:
    // let typeToRegMap = [type -> it's registration map] from initregs
    //   - keys are classTypes as well as interfaceTypes
    //   - if dict key conflict, error
    //
    // let remainingTypes = types where not typeToRegMap.ContainsKey
    //
    // for each concrete class in remainingTypes
    //   - 

    // OBSOLETE:
    // build [type -> implemented interfaces] dict
    // - first, take it from regs
    //   - if reg has interfaceTypes filled, take as they are
    //   - else, detect interfaces
    // - then, take from types
    // build registration list from the dict
    // apply lifetimes from initRegs
    // return registration list
    // TODO: ^ rethink, think of efficient building of the result list

    // - for each type, this type implements all implemented interfaces
    //   - if multiple classes implement given interface, do not register any of those classes as the interface's implementations
    initRegs
