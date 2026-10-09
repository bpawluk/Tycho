using Tycho.Identity;
using Tycho.Identity.Structure;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Identity.Structure;

public class InstanceIdentityTests
{
    [Fact]
    public void Create_WithoutSuffix_UsesTheTypeIdentity()
    {
        // Act
        InstanceIdentity identity = InstanceIdentity.Create(typeof(TestModule));

        // Assert
        Assert.Equal(TypeIdentifier.GetId(typeof(TestModule)), identity.Value);
        Assert.Equal(identity.Value, identity.ToString());
    }

    [Fact]
    public void Create_WithNullDefinitionType_ThrowsArgumentNullException()
    {
        // Act
        void Act() => InstanceIdentity.Create(null!);

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(Act);
        Assert.Equal("definitionType", exception.ParamName);
    }

    [Fact]
    public void Create_WithSuffix_AppendsItToTheTypeIdentity()
    {
        // Act
        InstanceIdentity identity = InstanceIdentity.Create(typeof(TestModule), "sales");

        // Assert
        Assert.Equal(TypeIdentifier.GetId(typeof(TestModule)) + ":sales", identity.Value);
    }

    [Fact]
    public void Create_WithMultipleSuffixes_PreservesPreviouslyConstructedInstances()
    {
        // Arrange
        string typeId = TypeIdentifier.GetId(typeof(TestModule));
        InstanceIdentity original = InstanceIdentity.Create(typeof(TestModule));

        // Act
        InstanceIdentity first = InstanceIdentity.Create(typeof(TestModule), "first");
        InstanceIdentity second = InstanceIdentity.Create(typeof(TestModule), "second");

        // Assert
        Assert.Equal(typeId, original.Value);
        Assert.Equal(typeId + ":first", first.Value);
        Assert.Equal(typeId + ":second", second.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Create_WithEmptyOrWhitespaceSuffix_ThrowsArgumentException(string suffix)
    {
        // Act
        void Act() => InstanceIdentity.Create(typeof(TestModule), suffix);

        // Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(Act);
        Assert.Equal("instanceSuffix", exception.ParamName);
    }

    [Fact]
    public void Create_WithDifferentSuffixes_ReturnsDifferentIdentities()
    {
        // Act
        InstanceIdentity sales = InstanceIdentity.Create(typeof(TestModule), "sales");
        InstanceIdentity support = InstanceIdentity.Create(typeof(TestModule), "support");

        // Assert
        Assert.NotEqual(sales, support);
    }

    [Fact]
    public void Create_WithSameDefinitionAndSuffix_ReturnsEqualInstances()
    {
        // Act
        InstanceIdentity first = InstanceIdentity.Create(typeof(TestModule), "leaf");
        InstanceIdentity replica = InstanceIdentity.Create(typeof(TestModule), "leaf");

        // Assert
        Assert.Equal(first, replica);
        Assert.NotSame(first, replica);
        Assert.Equal(first.GetHashCode(), replica.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentValueOrUnrelatedObject_ReturnsFalse()
    {
        // Arrange
        InstanceIdentity leaf = InstanceIdentity.Create(typeof(TestModule), "leaf");
        InstanceIdentity other = InstanceIdentity.Create(typeof(TestModule));

        // Act & Assert
        Assert.False(leaf.Equals(other));
        Assert.False(leaf.Equals((object)other));
        Assert.False(leaf.Equals(new object()));
        Assert.False(leaf.Equals(leaf.Value));
    }

    [Fact]
    public void Equality_WithNullReferences_EvaluatesCorrectly()
    {
        // Arrange
        InstanceIdentity identity = InstanceIdentity.Create(typeof(TestModule));
        InstanceIdentity? missing = null;

        // Act & Assert
        Assert.False(identity.Equals((InstanceIdentity?)null));
        Assert.False(identity.Equals((object?)null));
        Assert.False(identity == missing);
        Assert.False(missing == identity);
        Assert.True(identity != missing);
        Assert.True(missing != identity);
        Assert.True(missing == (InstanceIdentity?)null);
        Assert.False(missing != (InstanceIdentity?)null);
    }

    [Fact]
    public void Equals_WithDifferentCase_ReturnsFalse()
    {
        // Arrange
        InstanceIdentity first = InstanceIdentity.Parse("App");
        InstanceIdentity second = InstanceIdentity.Parse("app");

        // Act & Assert
        Assert.False(first.Equals(second));
        Assert.False(first == second);
    }

    [Theory]
    [InlineData("App")]
    [InlineData("Module:instance")]
    [InlineData("Generic<Argument>:instance")]
    [InlineData("UP")]
    [InlineData("DOWN(Module)/END")]
    public void Parse_WithOpaqueValue_PreservesItAndRoundTrips(string value)
    {
        // Act
        InstanceIdentity identity = InstanceIdentity.Parse(value);

        // Assert
        Assert.Equal(value, identity.Value);
        Assert.Equal(value, identity.ToString());
        Assert.Equal(identity, InstanceIdentity.Parse(identity.ToString()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Parse_WithMissingValue_ThrowsArgumentException(string? value)
    {
        // Act
        void Act() => InstanceIdentity.Parse(value!);

        // Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(Act);
        Assert.Equal("value", exception.ParamName);
    }

    private static readonly InstanceIdentity s_moduleIdentity = InstanceIdentity.Create(typeof(TestModule));

    public static readonly IEnumerable<object[]> EqualsTestData =
    [
        // Same instance => Equal
        [s_moduleIdentity, s_moduleIdentity, true],

        // Same module type => Equal
        [
            InstanceIdentity.Create(typeof(TestModule)),
            InstanceIdentity.Create(typeof(TestModule)),
            true
        ],

        // Different module types => Not Equal
        [
            InstanceIdentity.Create(typeof(TestModule)),
            InstanceIdentity.Create(typeof(OtherModule)),
            false
        ],

        // Comparing to null => Not Equal
        [InstanceIdentity.Create(typeof(TestModule)), null!, false]
    ];

    public static readonly IEnumerable<object[]> EqualsObjectTestData = EqualsTestData.Concat(
    [
        // Comparing to an object of a different type => Not Equal
        [InstanceIdentity.Create(typeof(TestModule)), new object(), false]
    ]);

    public static readonly IEnumerable<object[]> EqualsOperatorTestData = EqualsTestData.Concat(
    [
        // Comparing two null references => Equal
        [null!, null!, true],

        // Comparing null to an identity => Not Equal
        [null!, InstanceIdentity.Create(typeof(TestModule)), false]
    ]);

#pragma warning disable xUnit1042

    [Theory]
    [MemberData(nameof(EqualsTestData))]
    internal void InstanceIdentity_Equals_EvaluatesCorrectly(InstanceIdentity left, InstanceIdentity? right, bool areEqual)
    {
        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsObjectTestData))]
    internal void InstanceIdentity_EqualsObject_EvaluatesCorrectly(InstanceIdentity left, object? right, bool areEqual)
    {
        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void InstanceIdentity_EqualsOperator_EvaluatesCorrectly(InstanceIdentity? left, InstanceIdentity? right, bool areEqual)
    {
        // Act
        bool result = left == right;

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void InstanceIdentity_NotEqualsOperator_EvaluatesCorrectly(InstanceIdentity? left, InstanceIdentity? right, bool areEqual)
    {
        // Act
        bool result = left != right;

        // Assert
        Assert.Equal(!areEqual, result);
    }

#pragma warning restore xUnit1042
}
