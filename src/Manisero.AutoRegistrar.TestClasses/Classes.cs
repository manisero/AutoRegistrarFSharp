namespace Manisero.AutoRegistrar.TestClasses
{
    public class NoDependencies
    {
    }

    public class NoDependencies2
    {
    }

    public class DependantOf_NoDependencies
    {
        public DependantOf_NoDependencies(NoDependencies p1)
        {
        }
    }

    public class DependantOf_NoDependencies1And2
    {
        public DependantOf_NoDependencies1And2(NoDependencies p1, NoDependencies2 p2)
        {
        }
    }

    public class DependantOf_NoDependencies_x2
    {
        public DependantOf_NoDependencies_x2(NoDependencies p1, NoDependencies p2)
        {
        }
    }

    public class MultipleConstructors
    {
        public MultipleConstructors()
        {
        }

        public MultipleConstructors(NoDependencies p1)
        {
        }
    }
}
