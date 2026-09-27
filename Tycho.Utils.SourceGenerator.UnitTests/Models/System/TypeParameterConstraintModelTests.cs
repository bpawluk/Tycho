using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class TypeParameterConstraintModelTests
{
    private static readonly (TypeParameterConstraintModel Sut, string Expected)[] s_builtInConstraintCases =
    [
        (TypeParameterConstraintModel.ReferenceType, "class"),
        (TypeParameterConstraintModel.NullableReferenceType, "class?"),
        (TypeParameterConstraintModel.ValueType, "struct"),
        (TypeParameterConstraintModel.Unmanaged, "unmanaged"),
        (TypeParameterConstraintModel.NotNull, "notnull"),
        (TypeParameterConstraintModel.Constructor, "new()"),
        (TypeParameterConstraintModel.AllowsRefStruct, "allows ref struct")
    ];

    public static TheoryData<string> BuiltInConstraints
    {
        get
        {
            var data = new TheoryData<string>();
            foreach ((TypeParameterConstraintModel _, string Expected) in s_builtInConstraintCases)
            {
                data.Add(Expected);
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(BuiltInConstraints))]
    public void BuiltInConstraint_HasKeywordAndNoType(string expected)
    {
        // Arrange
        TypeParameterConstraintModel sut = s_builtInConstraintCases.Single(testCase => testCase.Expected == expected).Sut;

        // Act
        string result = sut.ToString();

        // Assert
        Assert.Equal(expected, sut.Keyword);
        Assert.Equal(expected, result);
        Assert.Null(sut.Type);
    }

    [Fact]
    public void TypeConstraint_StoresReference()
    {
        // Arrange
        TypeReferenceModel type = ModelHelpers.TypeReference("Base");

        // Act
        TypeParameterConstraintModel sut = TypeParameterConstraintModel.TypeConstraint(type);

        // Assert
        Assert.Equal("global::Example.Base", sut.Keyword);
        Assert.Equal(type, sut.Type);
        Assert.Equal("global::Example.Base", sut.ToString());
    }

    [Fact]
    public void Equality_EquivalentAndDifferentConstraints_UsesKeywordAndType()
    {
        // Arrange
        TypeParameterConstraintModel first = TypeParameterConstraintModel.ReferenceType;
        TypeParameterConstraintModel equal = TypeParameterConstraintModel.ReferenceType;
        TypeParameterConstraintModel different = TypeParameterConstraintModel.ValueType;

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
    public void Equality_EquivalentTypeConstraints_CompareTheirTypeReference()
    {
        TypeParameterConstraintModel first = TypeParameterConstraintModel.TypeConstraint(ModelHelpers.TypeReference("Base"));
        TypeParameterConstraintModel equal = TypeParameterConstraintModel.TypeConstraint(ModelHelpers.TypeReference("Base"));

        Assert.True(first.Equals(equal));
        Assert.Equal(first.GetHashCode(), equal.GetHashCode());
    }
}
