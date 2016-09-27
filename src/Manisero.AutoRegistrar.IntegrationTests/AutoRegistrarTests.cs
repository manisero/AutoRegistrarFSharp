using System;
using System.Collections.Generic;
using Manisero.AutoRegistrar.TestClasses;
using Xunit;

namespace Manisero.AutoRegistrar.IntegrationTests
{
    public class AutoRegistrarTests
    {
        [Fact]
        public void test()
        {
            var testClassesAssembly = typeof(R1).Assembly;
            var ignoredTypes = new HashSet<Type> { typeof(MultiCtors), typeof(SelfDependency), typeof(CyclicDependency1), typeof(CyclicDependency2) };

            var result = AutoRegistrar.FromRootAssemblyCSharp2(typeof(R2),
                                                               testClassesAssembly,
                                                               x => x == testClassesAssembly,
                                                               x => !ignoredTypes.Contains(x));
        }
    }
}
