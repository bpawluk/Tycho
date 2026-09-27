using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests.Models;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.Tycho;

public sealed class TychoDefinitionKindTests
{
    [Fact]
    public void Values_ContainsAllDefinitionKinds()
    {
        // Arrange
        TychoDefinitionKind[] expected = [TychoDefinitionKind.Unknown, TychoDefinitionKind.App, TychoDefinitionKind.Module];

        // Act
        TychoDefinitionKind[] result = Enum.GetValues<TychoDefinitionKind>();

        // Assert
        Assert.Equal(expected, result);
    }
}
