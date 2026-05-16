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
        string genericResult = TypeIdentifier.GetId<string>();
        string typeOverloadResult = TypeIdentifier.GetId(typeof(string));

        // Assert
        Assert.Equal(typeOverloadResult, genericResult);
    }

    [Theory]
    [MemberData(nameof(CommonTypes))]
    public void GetId_SameType_ReturnsSameId(Type type)
    {
        // Act
        string firstResult = TypeIdentifier.GetId(type);
        string secondResult = TypeIdentifier.GetId(type);

        // Assert
        Assert.Equal(firstResult, secondResult);
    }

    [Theory]
    [MemberData(nameof(DistinctTypePairs))]
    public void GetId_DifferentTypes_ReturnDifferentIds(Type first, Type second)
    {
        // Act
        string firstResult = TypeIdentifier.GetId(first);
        string secondResult = TypeIdentifier.GetId(second);

        // Assert
        Assert.NotEqual(firstResult, secondResult);
    }

    [Fact]
    public void GetId_ForNonGenericType_PrefixIsTypeName()
    {
        // Act
        string result = TypeIdentifier.GetId(typeof(string));

        // Assert
        string[] splited = result.Split("+");
        Assert.Equal(2, splited.Length);
        Assert.Equal("String", splited[0]);
    }

    [Fact]
    public void GetId_ForGenericType_PrefixIsTypeNameWithoutArity()
    {
        // Act
        string result = TypeIdentifier.GetId(typeof(List<int>));

        // Assert
        string[] splited = result.Split("+");
        Assert.Equal(2, splited.Length);
        Assert.Equal("List", splited[0]);
    }

    [Theory]
    [MemberData(nameof(CommonTypes))]
    public void GetId_SuffixIsEightAlphanumericCharacters(Type type)
    {
        // Act
        string result = TypeIdentifier.GetId(type);

        // Assert
        string[] splited = result.Split("+");
        Assert.Equal(2, splited.Length);

        string suffix = splited[1];
        Assert.Equal(8, suffix.Length);
        Assert.Matches(new Regex("^[0-9A-F]{8}$"), suffix);
    }
}
