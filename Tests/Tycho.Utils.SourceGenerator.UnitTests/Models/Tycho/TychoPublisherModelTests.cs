using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.Tycho;

public sealed class TychoPublisherModelTests
{
    [Fact]
    public void Constructor_SetsKindTypeAndEvents()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("Module");
        TypeReferenceModel eventType = ModelHelpers.TypeReference("Event");

        // Act
        var first = new TychoPublisherModel(TychoDefinitionKind.Module, definition, ModelHelpers.Items(eventType));

        // Assert
        Assert.Equal(TychoDefinitionKind.Module, first.DefinitionKind);
        Assert.Equal(definition, first.DefinitionType);
        Assert.Equal(eventType, Assert.Single(first.Events));
    }

    [Fact]
    public void Equality_UsesKindTypeAndEvents()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("Module");
        TypeReferenceModel eventType = ModelHelpers.TypeReference("Event");
        var first = new TychoPublisherModel(TychoDefinitionKind.Module, definition, ModelHelpers.Items(eventType));
        var equal = new TychoPublisherModel(TychoDefinitionKind.Module, ModelHelpers.TypeDefinition("Module"), ModelHelpers.Items(ModelHelpers.TypeReference("Event")));
        var different = new TychoPublisherModel(TychoDefinitionKind.App, definition, ModelHelpers.Items(eventType));

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
