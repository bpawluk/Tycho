using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.Tycho;

public sealed class TychoFacadeModelTests
{
    [Fact]
    public void Constructor_SetsKindTypeAndRequests()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("App");
        var request = new TychoRequestModel(ModelHelpers.TypeReference("Request"));

        // Act
        var first = new TychoFacadeModel(TychoDefinitionKind.App, definition, ModelHelpers.Items(request));

        // Assert
        Assert.Equal(TychoDefinitionKind.App, first.DefinitionKind);
        Assert.Equal(definition, first.DefinitionType);
        Assert.Equal(request, Assert.Single(first.Requests));
    }

    [Fact]
    public void Equality_UsesKindTypeAndRequests()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("App");
        var request = new TychoRequestModel(ModelHelpers.TypeReference("Request"));
        var first = new TychoFacadeModel(TychoDefinitionKind.App, definition, ModelHelpers.Items(request));
        var equal = new TychoFacadeModel(TychoDefinitionKind.App, ModelHelpers.TypeDefinition("App"), ModelHelpers.Items(new TychoRequestModel(ModelHelpers.TypeReference("Request"))));
        var different = new TychoFacadeModel(TychoDefinitionKind.Module, definition, ModelHelpers.Items(request));

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
