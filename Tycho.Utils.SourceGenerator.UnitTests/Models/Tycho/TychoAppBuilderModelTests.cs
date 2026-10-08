using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.Tycho;

public sealed class TychoAppBuilderModelTests
{
    [Fact]
    public void Constructor_SetsDefinitionType()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("App");

        // Act
        var first = new TychoAppBuilderModel(definition);

        // Assert
        Assert.Equal(definition, first.DefinitionType);
    }

    [Fact]
    public void Equality_UsesDefinitionType()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("App");
        var first = new TychoAppBuilderModel(definition);
        var equal = new TychoAppBuilderModel(ModelHelpers.TypeDefinition("App"));
        var different = new TychoAppBuilderModel(ModelHelpers.TypeDefinition("OtherApp"));

        // Act
        bool equalWithOperator = first == equal;
        bool unequalWithOperator = first != different;

        // Assert
        ModelHelpers.AssertValueSemantics(first, equal, different);
        Assert.True(equalWithOperator);
        Assert.True(unequalWithOperator);
        Assert.False(first != equal);
        Assert.False(first == different);
    }
}
