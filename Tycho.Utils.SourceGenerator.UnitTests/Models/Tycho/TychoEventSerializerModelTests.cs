using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.Tycho;

public sealed class TychoEventSerializerModelTests
{
    [Fact]
    public void Constructor_AndEquality_UseKindTypeAndEvents()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("App");
        TypeReferenceModel eventType = ModelHelpers.TypeReference("Event");
        var first = new TychoEventSerializerModel(TychoDefinitionKind.App, definition, ModelHelpers.Items(eventType));
        var equal = new TychoEventSerializerModel(TychoDefinitionKind.App, ModelHelpers.TypeDefinition("App"), ModelHelpers.Items(ModelHelpers.TypeReference("Event")));
        var different = new TychoEventSerializerModel(TychoDefinitionKind.Module, definition, ModelHelpers.Items(eventType));

        // Act
        bool equalWithOperator = first == equal;
        bool unequalWithOperator = first != different;

        // Assert
        Assert.Equal(TychoDefinitionKind.App, first.DefinitionKind);
        Assert.Equal(definition, first.DefinitionType);
        Assert.Equal(eventType, first.Events[0]);
        ModelHelpers.AssertValueSemantics(first, equal, different);
        Assert.True(equalWithOperator);
        Assert.True(unequalWithOperator);
        Assert.False(first != equal);
        Assert.False(first == different);
    }
}
