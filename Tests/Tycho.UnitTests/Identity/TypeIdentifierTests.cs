using System.Text.RegularExpressions;
using Tycho.Identity;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Identity;

public class TypeIdentifierTests
{
    public static readonly IEnumerable<object[]> CommonTypes =
    [
        [typeof(int)],
        [typeof(string)],
        [typeof(object)],
        [typeof(ValueTask)],
        [typeof(List<int>)]
    ];

    public static readonly IEnumerable<object[]> DistinctTypePairs =
    [
        [typeof(int),         typeof(string)],
        [typeof(float),       typeof(double)],
        [typeof(int),         typeof(List<int>)],
        [typeof(ValueTask),   typeof(Task)],
        [typeof(TestModule),  typeof(TestEvent)]
    ];

    [Fact]
    public void GenericGetId_ReturnsSameResultAsTypeOverload()
    {
        // Act
        var genericResult = TypeIdentifier.GetId<string>();
        var typeOverloadResult = TypeIdentifier.GetId(typeof(string));

        // Assert
        Assert.Equal(typeOverloadResult, genericResult);
    }

    [Theory]
    [MemberData(nameof(CommonTypes))]
    public void GetId_SameType_ReturnsSameId(Type type)
    {
        // Act
        var firstResult = TypeIdentifier.GetId(type);
        var secondResult = TypeIdentifier.GetId(type);

        // Assert
        Assert.Equal(firstResult, secondResult);
    }

    [Theory]
    [MemberData(nameof(DistinctTypePairs))]
    public void GetId_DifferentTypes_ReturnDifferentIds(Type first, Type second)
    {
        // Act
        var firstResult = TypeIdentifier.GetId(first);
        var secondResult = TypeIdentifier.GetId(second);

        // Assert
        Assert.NotEqual(firstResult, secondResult);
    }

    [Fact]
    public void GetId_ForNonGenericType_PrefixIsTypeName()
    {
        // Act
        var result = TypeIdentifier.GetId(typeof(string));

        // Assert
        var splited = result.Split("+");
        Assert.Equal(2, splited.Length);
        Assert.Equal("String", splited[0]);
    }

    [Fact]
    public void GetId_ForGenericType_PrefixIsTypeNameWithoutArity()
    {
        // Act
        var result = TypeIdentifier.GetId(typeof(List<int>));

        // Assert
        var splited = result.Split("+");
        Assert.Equal(2, splited.Length);
        Assert.Equal("List", splited[0]);
    }

    [Theory]
    [MemberData(nameof(CommonTypes))]
    public void GetId_SuffixIsEightAlphanumericCharacters(Type type)
    {
        // Act
        var result = TypeIdentifier.GetId(type);

        // Assert
        var splited = result.Split("+");
        Assert.Equal(2, splited.Length);

        var suffix = splited[1];
        Assert.Equal(8, suffix.Length);
        Assert.Matches(new Regex("^[0-9A-F]{8}$"), suffix);
    }
}
