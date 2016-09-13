namespace Manisero.AutoRegistrar

(*

Notes:
- initialMap contains predefined registrations
- single registration contains:
  - class
  - its lifetime
  - implemented interface(s) (optional)
- [nice to have] some registrations may only indicate class's lifetime without specifying implemented interface(s)

DiscoverAssemblies (() -> assemblies)
- get all referenced assemblies (accept some filter?)

ResolveDependencies (assemblies, initialMap -> classToInterfaceMap)
- scan assemblies for classes (accept some filter?)
- build classToInterfaceMap
  - respect initialMap
  - if multiple classes implement given interface, do not register any of those classes as the interface's implementations

BuildDependencyMap (classToInterfaceMap -> dependencyMap)
- for each class, resolve classes (not interfaces) it depends on
  - if interface with no implementation encountered, then exception
- TODO: handle cyclic dependencies (exception)

BuildDependencyGraph (dependencyMap -> dependencyGraph)
- (the graph is a directed acyclic graph, DAG)
- node in the graph: class (or registration)
- for given class, its dependencies (classes, not interfaces) are its parents
  - dependencies are constructor parameters
    - if more than one constructor, then exception

AssignDependancyLevels (dependencyGraph -> dependencyGraph)
- assign "dependancy level" to all classes (registrations)
- level = "depth" of the node in the graph
  - for each class with no dependencies, assign 0
  - then, for each class whose dependencies have level assigned (all of them), assign highest dependency level + 1
  - repeat above step until all levels are assigned

ResolveLifetimes (dependencyGraph, (initialMap?) -> registrations)
- order classes (registrations) by dependancyLevel
- for each class (ordered), resolve it's lifetime
  - respect initialMap
  - the class derives its lifetime from its shortest living dependency
  - if no dependencies, then longest possible lifetime

TODO:
- unify data types accepted / returned by functions
- immutability or mutability?

*)
