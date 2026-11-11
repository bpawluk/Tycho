using Tycho.Utils.SourceGenerator.Models.Tycho;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.Tycho;

public sealed class TychoDefinitionKindTests
{
    [Fact]
    public void Values_ContainsAllDefinitionKinds()
    {
        // Act
        TychoDefinitionKind[] result = Enum.GetValues<TychoDefinitionKind>();

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal(TychoDefinitionKind.Unknown, result[0]);
        Assert.Equal(TychoDefinitionKind.App, result[1]);
        Assert.Equal(TychoDefinitionKind.Module, result[2]);
    }
}
