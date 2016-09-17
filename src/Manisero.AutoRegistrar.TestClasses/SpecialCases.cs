namespace Manisero.AutoRegistrar.TestClasses
{
    public interface IMultiImpls { }
    public class MultiImpl1 : IMultiImpls { }
    public class MultiImpl2 : IMultiImpls { }

    public class MultiCtors
    {
        public MultiCtors() {}
        public MultiCtors(R1 p1) {}
    }

    public class SelfDependency
    {
        public SelfDependency(SelfDependency p1) { }
    }

    public class CyclicDependency1 { }
    public class CyclicDependency2 { }
}
