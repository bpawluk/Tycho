using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.Tycho;

public sealed class TychoExtensionsModelTests
{
    [Fact]
    public void Constructor_AndEquality_UseDefinitionType()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("App");
        var first = new TychoExtensionsModel(definition);
        var equal = new TychoExtensionsModel(ModelHelpers.TypeDefinition("App"));
        var different = new TychoExtensionsModel(ModelHelpers.TypeDefinition("OtherApp"));

        // Act
        bool equalWithOperator = first == equal;
        bool unequalWithOperator = first != different;

        // Assert
        Assert.Equal(definition, first.DefinitionType);
        ModelHelpers.AssertValueSemantics(first, equal, different);
        Assert.True(equalWithOperator);
        Assert.True(unequalWithOperator);
        Assert.False(first != equal);
        Assert.False(first == different);
    }
}
