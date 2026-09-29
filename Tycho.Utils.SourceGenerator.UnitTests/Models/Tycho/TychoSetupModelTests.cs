using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.Tycho;

public sealed class TychoSetupModelTests
{
    [Fact]
    public void Constructor_SetsKindTypeAndSubmodules()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("App");
        TypeReferenceModel submodule = ModelHelpers.TypeReference("Module");

        // Act
        var first = new TychoSetupModel(TychoDefinitionKind.App, definition, ModelHelpers.Items(submodule));

        // Assert
        Assert.Equal(TychoDefinitionKind.App, first.DefinitionKind);
        Assert.Equal(definition, first.DefinitionType);
        Assert.Equal(submodule, Assert.Single(first.Submodules));
    }

    [Fact]
    public void Equality_UsesKindTypeAndSubmodules()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("App");
        TypeReferenceModel submodule = ModelHelpers.TypeReference("Module");
        var first = new TychoSetupModel(TychoDefinitionKind.App, definition, ModelHelpers.Items(submodule));
        var equal = new TychoSetupModel(TychoDefinitionKind.App, ModelHelpers.TypeDefinition("App"), ModelHelpers.Items(ModelHelpers.TypeReference("Module")));
        var different = new TychoSetupModel(TychoDefinitionKind.Module, definition, ModelHelpers.Items(submodule));

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
