using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Manisero.AutoRegistrar.Domain;
using Manisero.AutoRegistrar.TestClasses;
using Xunit;

namespace Manisero.AutoRegistrar.IntegrationTests
{
    public class AutoRegistrarTests
    {
        [Theory]
        [InlineData(typeof(R1), new[] { typeof(IR1) }, new Type[0], 0, 1)]
        [InlineData(typeof(R2), new[] { typeof(R2_Base), typeof(IR2_Base), typeof(IR2_1), typeof(IR2_2) }, new Type[0], 0, 3)]
        [InlineData(typeof(C1A_R1), new Type[0], new[] {typeof(R1) }, 1, 1)]
        [InlineData(typeof(C1A_IR1), new Type[0], new[] { typeof(R1) }, 1, 1)]
        public void resolves_registrations_from_root_assembly(Type testedType, Type[] expectedInterfaces, Type[] expectedDependencyTypes, int expectedDependancyLevel, int expectedLifetime)
        {
            // Arrange
            var initialRegistrations = new List<Registration>
                {
                    new Registration(typeof(R2)) { Lifetime = 3 },
                    new Registration(typeof(C1A_R1)) { InterfaceTypes = new List<Type>() },
                    new Registration(typeof(C1A_IR1)) { InterfaceTypes = new List<Type>() }
                };

            var rootAssembly = typeof(R1).Assembly;

            Func<Assembly, bool> assemblyFilter = x => x == rootAssembly;

            var ignoredTypes = new HashSet<Type> { typeof(MultiCtors), typeof(SelfDependency), typeof(CyclicDependency1), typeof(CyclicDependency2) };
            Func<Type, bool> typeFilter = x => !ignoredTypes.Contains(x);

            // Act
            var result = AutoRegistrar.FromRootAssembly(initialRegistrations, rootAssembly, assemblyFilter, typeFilter);

            // Assert
            var testedRegistration = result.Single(x => x.ClassType == testedType);
            testedRegistration.InterfaceTypes.ShouldAllBeEquivalentTo(expectedInterfaces);
            testedRegistration.Dependencies.Select(x => x.ClassType).ShouldAllBeEquivalentTo(expectedDependencyTypes);
            testedRegistration.DependancyLevel.Should().Be(expectedDependancyLevel);
            testedRegistration.Lifetime.Should().Be(expectedLifetime);
        }
    }
}
