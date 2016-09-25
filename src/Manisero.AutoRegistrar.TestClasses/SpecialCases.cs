using System;
using System.Reflection;

namespace Manisero.AutoRegistrar.TestClasses
{
    public static class ReferencedByTestClassesOnly
    {
        public static readonly Type Type = typeof(AutoRegistrar.ReferencedByTestClassesOnly.ReferencedByTestClassesOnly);
        public static readonly Assembly Assembly = Type.Assembly;
    }

    public class NoInters { }

    public interface IMultiImpls { }
    public class MultiImpl1 : IMultiImpls { }
    public class MultiImpl2 : IMultiImpls { }

    public interface IMultiImpls2_1 { }
    public interface IMultiImpls2_2 { }
    public interface IMultiImpls2_3 { }
    public interface IMultiImpl2_1 { }
    public class MultiImpl2_1 : IMultiImpl2_1, IMultiImpls2_1, IMultiImpls2_2, IMultiImpls2_3 { }
    public interface IMultiImpl2_2 { }
    public class MultiImpl2_2 : IMultiImpl2_2, IMultiImpls2_1, IMultiImpls2_2, IMultiImpls2_3 { }

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
