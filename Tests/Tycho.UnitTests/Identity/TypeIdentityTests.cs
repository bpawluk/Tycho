using Tycho.Identity;
using Tycho.Identity.Structure;

namespace Tycho.UnitTests.Identity;

public sealed class TypeIdentityTests
{
    [Theory]
    [InlineData("Example.Type", true)]
    [InlineData("Example.OtherType", false)]
    [InlineData(null, false)]
    public void Equals_WithTypedIdentity_ComparesValues(string? otherValue, bool expected)
    {
        // Arrange
        TypeIdentity sut = DefinitionIdentity.Parse("Example.Type");
        TypeIdentity? other = otherValue is null ? null : DefinitionIdentity.Parse(otherValue);

        // Act
        bool result = sut.Equals(other);

        // Assert
        Assert.Equal(expected, result);
        Assert.Equal(sut == other, result);
    }
}
