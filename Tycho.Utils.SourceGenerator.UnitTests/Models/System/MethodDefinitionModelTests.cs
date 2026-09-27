using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class MethodDefinitionModelTests
{
    [Fact]
    public void Constructor_NullBody_UsesEmptyBody()
    {
        // Arrange
        TypeDefinitionModel containingType = ModelHelpers.TypeDefinition("Owner");
        MethodSignatureModel signature = ModelHelpers.MethodSignature();

        // Act
        var sut = new MethodDefinitionModel(containingType, signature, null);

        // Assert
        Assert.Equal(containingType, sut.ContainingType);
        Assert.Equal(signature, sut.Signature);
        Assert.Empty(sut.Body);
    }

    [Fact]
    public void Equality_EquivalentAndDifferentDefinitions_UsesContainingTypeSignatureAndBody()
    {
        // Arrange
        TypeDefinitionModel containingType = ModelHelpers.TypeDefinition("Owner");
        MethodSignatureModel signature = ModelHelpers.MethodSignature();
        MethodInvocationModel invocation = new(signature, null, null);
        var first = new MethodDefinitionModel(containingType, signature, ModelHelpers.Items(invocation));
        var equal = new MethodDefinitionModel(containingType, signature, ModelHelpers.Items(invocation));
        var different = new MethodDefinitionModel(containingType, signature, ModelHelpers.Items(new MethodInvocationModel(ModelHelpers.MethodSignature("Stop"), null, null)));

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
