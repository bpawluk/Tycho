using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class TypeModifierTests
{
    private static readonly (TypeModifier Sut, string Expected)[] s_modifierCases =
    [
        (TypeModifier.New, "new"),
        (TypeModifier.Public, "public"),
        (TypeModifier.Protected, "protected"),
        (TypeModifier.Internal, "internal"),
        (TypeModifier.Private, "private"),
        (TypeModifier.File, "file"),
        (TypeModifier.ProtectedInternal, "protected internal"),
        (TypeModifier.PrivateProtected, "private protected"),
        (TypeModifier.Static, "static"),
        (TypeModifier.Virtual, "virtual"),
        (TypeModifier.Sealed, "sealed"),
        (TypeModifier.Override, "override"),
        (TypeModifier.Abstract, "abstract"),
        (TypeModifier.Extern, "extern"),
        (TypeModifier.Const, "const"),
        (TypeModifier.Event, "event"),
        (TypeModifier.Fixed, "fixed"),
        (TypeModifier.ReadOnly, "readonly"),
        (TypeModifier.Ref, "ref"),
        (TypeModifier.In, "in"),
        (TypeModifier.Out, "out"),
        (TypeModifier.Params, "params"),
        (TypeModifier.This, "this"),
        (TypeModifier.Scoped, "scoped"),
        (TypeModifier.Unsafe, "unsafe"),
        (TypeModifier.Volatile, "volatile"),
        (TypeModifier.Async, "async"),
        (TypeModifier.Partial, "partial"),
        (TypeModifier.Required, "required")
    ];

    public static TheoryData<string> Modifiers
    {
        get
        {
            var data = new TheoryData<string>();
            foreach ((TypeModifier _, string Expected) in s_modifierCases)
            {
                data.Add(Expected);
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(Modifiers))]
    public void Keyword_ReturnsExpectedValue(string expected)
    {
        // Arrange
        TypeModifier sut = s_modifierCases.Single(testCase => testCase.Expected == expected).Sut;

        // Act
        string result = sut.Keyword;

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(Modifiers))]
    public void ToString_ReturnsKeyword(string expected)
    {
        // Arrange
        TypeModifier sut = s_modifierCases.Single(testCase => testCase.Expected == expected).Sut;

        // Act
        string result = sut.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Equality_EquivalentAndDifferentModifiers_UsesKeyword()
    {
        // Arrange
        TypeModifier first = TypeModifier.Public;
        TypeModifier equal = TypeModifier.Public;
        TypeModifier different = TypeModifier.Private;

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
