namespace Manisero.AutoRegistrar.TestClasses
{
    public class NoInters { }

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

    public class CyclicDependency1
    {
        public CyclicDependency1(CyclicDependency2 p1) { }
    }

    public class CyclicDependency2
    {
        public CyclicDependency2(CyclicDependency1 p1) { }
    }
}
