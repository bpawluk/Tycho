using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class MethodInvocationModelTests
{
    [Fact]
    public void Constructor_NullArguments_UsesEmptyArguments()
    {
        // Arrange
        MethodSignatureModel signature = ModelHelpers.MethodSignature();

        // Act
        var sut = new MethodInvocationModel(signature, receiverType: null, typeArguments: null);

        // Assert
        Assert.Equal(signature, sut.Signature);
        Assert.Null(sut.ReceiverType);
        Assert.Empty(sut.TypeArguments);
    }

    [Fact]
    public void Equality_NullReceivers_AreEqual()
    {
        // Arrange
        MethodSignatureModel signature = ModelHelpers.MethodSignature();
        var first = new MethodInvocationModel(signature, null, null);
        var equal = new MethodInvocationModel(signature, null, ModelHelpers.Items<TypeArgumentModel>());
        var different = new MethodInvocationModel(signature, ModelHelpers.TypeReference("Receiver"), null);

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

    [Fact]
    public void Constructor_SetsSignatureReceiverAndTypeArguments()
    {
        // Arrange
        MethodSignatureModel signature = ModelHelpers.MethodSignature();
        TypeReferenceModel receiver = ModelHelpers.TypeReference("Receiver");
        var typeArgument = new TypeArgumentModel("T", ModelHelpers.TypeReference("Value"));

        // Act
        var first = new MethodInvocationModel(signature, receiver, ModelHelpers.Items(typeArgument));

        // Assert
        Assert.Equal(signature, first.Signature);
        Assert.Equal(receiver, first.ReceiverType);
        Assert.Equal(typeArgument, Assert.Single(first.TypeArguments));
    }

    [Fact]
    public void Equality_ReceiverAndTypeArguments_AffectEquality()
    {
        // Arrange
        MethodSignatureModel signature = ModelHelpers.MethodSignature();
        TypeReferenceModel receiver = ModelHelpers.TypeReference("Receiver");
        var typeArgument = new TypeArgumentModel("T", ModelHelpers.TypeReference("Value"));
        var first = new MethodInvocationModel(signature, receiver, ModelHelpers.Items(typeArgument));
        var equal = new MethodInvocationModel(signature, receiver, ModelHelpers.Items(typeArgument));
        var different = new MethodInvocationModel(signature, receiver, ModelHelpers.Items(new TypeArgumentModel("U", typeArgument.Value)));

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
