using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class MethodSignatureModelTests
{
    [Fact]
    public void Constructor_SetsMethodShape()
    {
        // Arrange
        TypeReferenceModel parameter = ModelHelpers.TypeReference("Input");
        TypeReferenceModel resultType = ModelHelpers.TypeReference("Result");
        var sut = new MethodSignatureModel("Run", ModelHelpers.Items(parameter), resultType);
        // Assert
        Assert.Equal("Run", sut.MethodName);
        Assert.Equal(parameter, Assert.Single(sut.Parameters));
        Assert.Equal(resultType, sut.Result);
    }

    [Fact]
    public void Matches_ComparesMethodShape()
    {
        // Arrange
        TypeReferenceModel parameter = ModelHelpers.TypeReference("Input");
        TypeReferenceModel resultType = ModelHelpers.TypeReference("Result");
        var sut = new MethodSignatureModel("Run", ModelHelpers.Items(parameter), resultType);
        var equal = new MethodSignatureModel("Run", ModelHelpers.Items(parameter), resultType);
        var different = new MethodSignatureModel("Run", ModelHelpers.Items(ModelHelpers.TypeReference("Other")), resultType);

        // Act
        bool matchesEqual = sut.Matches(equal);
        bool matchesDifferent = sut.Matches(different);

        // Assert
        Assert.True(matchesEqual);
        Assert.False(matchesDifferent);
    }

    [Fact]
    public void Constructor_NullNameAndParameters_UsesEmptyValues()
    {
        // Arrange
        TypeReferenceModel resultType = ModelHelpers.TypeReference("Result");

        // Act
        var sut = new MethodSignatureModel(null, null, resultType);

        // Assert
        Assert.Empty(sut.MethodName);
        Assert.Empty(sut.Parameters);
        Assert.Equal(resultType, sut.Result);
    }

    [Fact]
    public void Matches_TreatsNullNameAsEmpty()
    {
        // Arrange
        TypeReferenceModel resultType = ModelHelpers.TypeReference("Result");
        var sut = new MethodSignatureModel(null, null, resultType);

        // Assert
        Assert.True(sut.Matches(new MethodSignatureModel(string.Empty, null, resultType)));
    }

    [Fact]
    public void Equality_EquivalentAndDifferentSignatures_UsesMethodShape()
    {
        // Arrange
        MethodSignatureModel first = ModelHelpers.MethodSignature();
        MethodSignatureModel equal = ModelHelpers.MethodSignature();
        MethodSignatureModel different = ModelHelpers.MethodSignature(name: "Stop");

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
