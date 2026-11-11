using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class TypeParameterModelTests
{
    [Fact]
    public void ConstraintsClause_FormatsConstraintsInOrder()
    {
        // Arrange
        var sut = new TypeParameterModel(
            "T",
            ModelHelpers.Items(
                TypeParameterConstraintModel.ReferenceType,
                TypeParameterConstraintModel.Constructor));

        // Act
        string result = sut.ConstraintsClause;

        // Assert
        Assert.Equal("T", sut.Name);
        Assert.Equal(2, sut.Constraints.Count);
        Assert.Equal("where T : class, new()", result);
    }

    [Fact]
    public void Constructor_NullNameAndConstraints_UsesEmptyValues()
    {
        // Arrange
        var sut = new TypeParameterModel(null, null);

        // Act
        string result = sut.ConstraintsClause;

        // Assert
        Assert.Empty(sut.Name);
        Assert.Empty(sut.Constraints);
        Assert.Empty(result);
    }

    [Fact]
    public void Equality_EquivalentAndDifferentParameters_UsesNameAndConstraints()
    {
        // Arrange
        var first = new TypeParameterModel("T", ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType));
        var equal = new TypeParameterModel("T", ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType));
        var different = new TypeParameterModel("U", ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType));

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
