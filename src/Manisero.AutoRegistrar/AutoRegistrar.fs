namespace Manisero.AutoRegistrar

(*

Notes:
- initialMap contains predefined registrations
- single registration contains:
  - class
  - its lifetime
  - implemented interface(s) (optional)
- [nice to have] some registrations may only indicate class's lifetime without specifying implemented interface(s)

DiscoverAndResolve (initialMap -> registrations)
- get all referenced assemblies (accept some filter?)
- invoke MapAndResolve(assemblies)

MapAndResolve (initialMap, assemblies -> registrations)
- scan assemblies for classes (accept some filter?)
- build classToInterfaceMap based on initialMap
  - if multiple classes implement given interface, do not register any of those classes as the interface's implementations
- invoke Resolve(map)

Resolve (classToInterfaceMap, (initialMap?) -> registrations)
- build dependency tree (actually, a directed acyclic graph, DAG)
  - node in the tree: class (or registration)
  - for given class, its dependencies are its parents
    - dependencies are constructor parameters
      - if more than one constructor, then exception
  - assign "dependancy level" to all classes (registrations)
    - level = depth of the node in the "tree"
      - for each class with no dependencies, assign 0
      - then, for each class whose dependencies have level assigned (all of them), assign highest dependency level + 1
      - repeat above step until all levels are assigned
- order classes (registrations) by dependancyLevel
- for each class (ordered), resolve it's lifetime
  - respect initialMap
  - the class derives its lifetime from its shortest living dependency
  - if no dependencies, then longest possible lifetime

*)
