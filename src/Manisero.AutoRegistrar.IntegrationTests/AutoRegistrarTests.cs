using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manisero.AutoRegistrar.Domain;
using Manisero.AutoRegistrar.TestClasses;
using Xunit;

namespace Manisero.AutoRegistrar.IntegrationTests
{
    public class AutoRegistrarTests
    {
        [Theory]
        [InlineData()]
        public void resolves_registrations_from_root_assembly(Type testedType, Type[] expectedInterfaces, Type[] expectedDependencyTypes, int expectedDependancyLevel, int expectedLifetime)
        {
            // Arrange
            var initialRegistrations = new List<Registration> { new Registration(typeof(R2)) { Lifetime = 3 } };
            var rootAssembly = typeof(R1).Assembly;
            
            Func<Assembly, bool> assemblyFilter = x => x == rootAssembly;

            var ignoredTypes = new HashSet<Type> { typeof(MultiCtors), typeof(SelfDependency), typeof(CyclicDependency1), typeof(CyclicDependency2) };
            Func<Type, bool> typeFilter = x => !ignoredTypes.Contains(x);

            // Act
            var result = AutoRegistrar.FromRootAssembly(initialRegistrations, rootAssembly, assemblyFilter, typeFilter);

            // Assert
            var testedRegistration = result.Single(x => x.ClassType == testedType);
            
        }
    }
}
