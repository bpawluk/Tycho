using Tycho.Identity.Modules;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Identity.Modules;

public class ModuleIdentityTests
{
    private static readonly ModuleIdentity s_moduleIdentity = ModuleIdentity.Create<TestModule>();

    public static readonly IEnumerable<object[]> EqualsTestData =
    [
        // Same instance => Equal
        [s_moduleIdentity, s_moduleIdentity, true],

        // Same module type => Equal
        [
            ModuleIdentity.Create<TestModule>(),
            ModuleIdentity.Create<TestModule>(),
            true
        ],

        // Different module types => Not Equal
        [
            ModuleIdentity.Create<TestModule>(),
            ModuleIdentity.Create<OtherModule>(),
            false
        ],

        // Comparing to null => Not Equal
        [ModuleIdentity.Create<TestModule>(), null!, false]
    ];

    public static readonly IEnumerable<object[]> EqualsObjectTestData = EqualsTestData.Concat(
    [
        // Comparing to an object of a different type => Not Equal
        [ModuleIdentity.Create<TestModule>(), new object(), false]
    ]);

    public static readonly IEnumerable<object[]> EqualsOperatorTestData = EqualsTestData.Concat(
    [
        // Comparing two null references => Equal
        [null!, null!, true],

        // Comparing null to an identity => Not Equal
        [null!, ModuleIdentity.Create<TestModule>(), false]
    ]);

#pragma warning disable xUnit1042

    [Theory]
    [MemberData(nameof(EqualsTestData))]
    internal void ModuleIdentity_Equals_EvaluatesCorrectly(ModuleIdentity left, ModuleIdentity? right, bool areEqual)
    {
        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsObjectTestData))]
    internal void ModuleIdentity_EqualsObject_EvaluatesCorrectly(ModuleIdentity left, object? right, bool areEqual)
    {
        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void ModuleIdentity_EqualsOperator_EvaluatesCorrectly(ModuleIdentity? left, ModuleIdentity? right, bool areEqual)
    {
        // Act
        bool result = left == right;

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void ModuleIdentity_NotEqualsOperator_EvaluatesCorrectly(ModuleIdentity? left, ModuleIdentity? right, bool areEqual)
    {
        // Act
        bool result = left != right;

        // Assert
        Assert.Equal(!areEqual, result);
    }

#pragma warning restore xUnit1042
}
