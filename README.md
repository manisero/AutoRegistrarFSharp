# AutoRegistrarFSharp

Automatic DI Container registrations resolver.



# Notes:

- initialMap contains predefined registrations
- single registration contains:
  - class
  - its lifetime
  - implemented interface(s) (optional)
- Registration record contains mutable fields in order to avoid:
  - memory pressure (copying Registration record to preserve immutability)
  - splitting Registration record into set of [class -> field] hashmaps
    - this would avoid copying records, but imply keeping Registartion state in 4-5 hashmaps
- [nice to have] some registrations may only indicate class's lifetime without specifying implemented interface(s)

----------

**DiscoverAssemblies** (() -> assemblies)

- get all referenced assemblies (accept some filter?)

----------

**ResolveDependencies** (assemblies, initialMap -> classToInterfaceMap)

- scan assemblies for classes (accept some filter?)
- build classToInterfaceMap
  - respect initialMap
  - if multiple classes implement given interface, do not register any of those classes as the interface's implementations

----------

**BuildDependencyGraph** (Registration list -> unit)

- (the graph is a directed acyclic graph, DAG)
- node in the graph: class (or registration)
- for given class, its dependencies (classes, not interfaces) are its parents
  - dependencies are constructor parameters
    - if more than one constructor, then exception

----------

**AssignDependancyLevels** (Registration list -> unit)

- dependancyLevel = "depth" of the node in the graph
- assign dependancyLevel to all classes (registrations)
  - for each class with no dependencies, assign 0
  - then, for each class whose dependencies have level assigned (all of them), assign highest dependency level + 1
  - repeat above step until all levels are assigned
  - if cyclic dependency encountered, then exception

----------

**ResolveLifetimes** (Registration list -> unit)

- order classes (registrations) by dependancyLevel
- for each class (ordered), resolve it's lifetime
  - respect initialMap
  - the class derives its lifetime from its shortest living dependency
  - if no dependencies, then longest possible lifetime
