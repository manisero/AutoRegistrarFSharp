module ImplementationMap

open System

let getClassInterfaces (typ:Type) =
    if (typ.BaseType = typeof<Object>)
    then Array.toList (typ.GetInterfaces())
    else typ.BaseType :: Array.toList (typ.GetInterfaces())

let handleInterType handledTypes typeToRegMap reg inter = null

let buildImplementationMap buildTypesSet buildTypeToRegMap getClassInterfaces handleInterType regs =
    // regs contains all registrations to fill (fill = set interfaceTypes)
    // each reg can contain lifetime, but should not contain interfaceTypes
    
    // for each reg:
    // - if classType is interface or is abstract, error
    // - it's interfaces are interfaces it implements and base class it derives
    //   - TODO: consider walking through full type hierarchy
    // if multiple classes implement given interface, then this interface should not appear in any of those classes' interfaceTypes
    // - e.g. IDisposable can be implemented by multiple classes
    // if a base class of given class is any reg's classType, then the base class should not appear in the class's interfaceTypes

    // steps:
    // 1. let handledTypes = hashset of all types present in regs (classTypes and interfaceTypes)
    //   - if any type occurs multiple times, error
    // 2. let typeToRegMap = [type -> it's registration] map from regs
    // 3. for each reg.classType where interfaceTypes is null
    //   - for each of class's interfaces
    //     - if interface is present in handledTypes, continue
    //     - else if interface is present in typeToRegMap.keys
    //       - remove it from its registration interfaceTypes
    //       - remove it from typeToRegMap
    //       - add it to handledTypes
    //     - else
    //       - add it to class's interfaceTypes
    //       - add it to typeToRegMap
    // 4. return
    ignore null
