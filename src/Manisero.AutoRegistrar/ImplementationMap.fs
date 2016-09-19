module ImplementationMap

let buildImplementationMap initRegs types =
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
