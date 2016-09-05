namespace Manisero.AutoRegistrar.TestClasses
{
    public class NoDependencies
    {
    }

    public class DependantOf_NoDependencies
    {
        public DependantOf_NoDependencies(NoDependencies noDependencies)
        {
        }
    }

    public class MultipleConstructors
    {
        public MultipleConstructors()
        {
        }

        public MultipleConstructors(NoDependencies noDependencies)
        {
        }
    }
}
