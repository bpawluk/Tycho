using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class TypeKindTests
{
    private static readonly (TypeKind Sut, string Expected)[] s_kindCases =
    [
        (TypeKind.Class, "class"),
        (TypeKind.RecordClass, "record class"),
        (TypeKind.Struct, "struct"),
        (TypeKind.RecordStruct, "record struct"),
        (TypeKind.Interface, "interface"),
        (TypeKind.Enum, "enum"),
        (TypeKind.Other, "other")
    ];

    public static TheoryData<string> Kinds
    {
        get
        {
            var data = new TheoryData<string>();
            foreach ((TypeKind _, string Expected) in s_kindCases)
            {
                data.Add(Expected);
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(Kinds))]
    public void Keyword_AndToString_ReturnExpectedValue(string expected)
    {
        // Arrange
        TypeKind sut = s_kindCases.Single(testCase => testCase.Expected == expected).Sut;

        // Act
        string keyword = sut.Keyword;
        string result = sut.ToString();

        // Assert
        Assert.Equal(expected, keyword);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Equality_EquivalentAndDifferentKinds_UsesKeyword()
    {
        // Arrange
        TypeKind first = TypeKind.Class;
        TypeKind equal = TypeKind.Class;
        TypeKind different = TypeKind.Struct;

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
