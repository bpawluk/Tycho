using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.Tycho;

public sealed class TychoParentModelTests
{
    [Fact]
    public void Constructor_SetsDefinitionTypeAndRequests()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("Module");
        var request = new TychoRequestModel(ModelHelpers.TypeReference("Request"));

        // Act
        var first = new TychoParentModel(definition, ModelHelpers.Items(request));

        // Assert
        Assert.Equal(definition, first.DefinitionType);
        Assert.Equal(request, Assert.Single(first.Requests));
    }

    [Fact]
    public void Equality_UsesDefinitionTypeAndRequests()
    {
        // Arrange
        TypeDefinitionModel definition = ModelHelpers.TypeDefinition("Module");
        var request = new TychoRequestModel(ModelHelpers.TypeReference("Request"));
        var first = new TychoParentModel(definition, ModelHelpers.Items(request));
        var equal = new TychoParentModel(ModelHelpers.TypeDefinition("Module"), ModelHelpers.Items(new TychoRequestModel(ModelHelpers.TypeReference("Request"))));
        var different = new TychoParentModel(definition, ModelHelpers.Items(new TychoRequestModel(ModelHelpers.TypeReference("OtherRequest"))));
        var differentDefinition = new TychoParentModel(ModelHelpers.TypeDefinition("OtherModule"), ModelHelpers.Items(request));

        // Act
        bool equalWithOperator = first == equal;
        bool unequalWithOperator = first != different;

        // Assert
        ModelHelpers.AssertValueSemantics(first, equal, different);
        Assert.True(equalWithOperator);
        Assert.True(unequalWithOperator);
        Assert.False(first != equal);
        Assert.False(first == different);
        Assert.False(first.Equals(differentDefinition));
    }
}
