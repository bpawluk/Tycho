using Tycho.Identity.Structure;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Identity.Structure;

public class DefinitionIdentityTests
{
    private static readonly DefinitionIdentity s_moduleIdentity = DefinitionIdentity.Create<TestModule>();

    public static readonly IEnumerable<object[]> EqualsTestData =
    [
        // Same instance => Equal
        [s_moduleIdentity, s_moduleIdentity, true],

        // Same module type => Equal
        [
            DefinitionIdentity.Create<TestModule>(),
            DefinitionIdentity.Create<TestModule>(),
            true
        ],

        // Different module types => Not Equal
        [
            DefinitionIdentity.Create<TestModule>(),
            DefinitionIdentity.Create<OtherModule>(),
            false
        ],

        // Comparing to null => Not Equal
        [DefinitionIdentity.Create<TestModule>(), null!, false]
    ];

    public static readonly IEnumerable<object[]> EqualsObjectTestData = EqualsTestData.Concat(
    [
        // Comparing to an object of a different type => Not Equal
        [DefinitionIdentity.Create<TestModule>(), new object(), false]
    ]);

    public static readonly IEnumerable<object[]> EqualsOperatorTestData = EqualsTestData.Concat(
    [
        // Comparing two null references => Equal
        [null!, null!, true],

        // Comparing null to an identity => Not Equal
        [null!, DefinitionIdentity.Create<TestModule>(), false]
    ]);

#pragma warning disable xUnit1042

    [Theory]
    [MemberData(nameof(EqualsTestData))]
    internal void DefinitionIdentity_Equals_EvaluatesCorrectly(DefinitionIdentity left, DefinitionIdentity? right, bool areEqual)
    {
        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsObjectTestData))]
    internal void DefinitionIdentity_EqualsObject_EvaluatesCorrectly(DefinitionIdentity left, object? right, bool areEqual)
    {
        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void DefinitionIdentity_EqualsOperator_EvaluatesCorrectly(DefinitionIdentity? left, DefinitionIdentity? right, bool areEqual)
    {
        // Act
        bool result = left == right;

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void DefinitionIdentity_NotEqualsOperator_EvaluatesCorrectly(DefinitionIdentity? left, DefinitionIdentity? right, bool areEqual)
    {
        // Act
        bool result = left != right;

        // Assert
        Assert.Equal(!areEqual, result);
    }

#pragma warning restore xUnit1042
}
