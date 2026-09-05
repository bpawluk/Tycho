using System.Text.RegularExpressions;
using Tycho.Identity;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Identity;

public partial class TypeIdentifierTests
{
    public static readonly TheoryData<Type, string> TypesWithTemplates = new()
    {
        { typeof(int), "Int32+HASH" },
        { typeof(string), "String+HASH" },
        { typeof(object), "Object+HASH" },
        { typeof(List<int>), "List+HASH<Int32+HASH>" },
        { typeof(ValueTask<int?>), "ValueTask+HASH<Nullable+HASH<Int32+HASH>>" },
        { typeof(Dictionary<string, int>), "Dictionary+HASH<String+HASH,Int32+HASH>" },
        { typeof(GenericModule<Tuple<int, DateTime?>, string>), "GenericModule+HASH<Tuple+HASH<Int32+HASH,Nullable+HASH<DateTime+HASH>>,String+HASH>" },
        { typeof(GenericModule<,>), "GenericModule+HASH<T,Q>" }
    };

    public static readonly TheoryData<Type, Type> DistinctTypePairs = new()
    {
        { typeof(int),        typeof(string) },
        { typeof(float),      typeof(double) },
        { typeof(int),        typeof(List<int>) },
        { typeof(ValueTask),  typeof(Task) },
        { typeof(TestModule), typeof(TestEvent) }
    };

    [Fact]
    public void GenericGetId_ReturnsSameResultAsTypeOverload()
    {
        // Act
        string genericResult = TypeIdentifier.GetId<string>();
#pragma warning disable CA2263
        string typeOverloadResult = TypeIdentifier.GetId(typeof(string));
#pragma warning restore CA2263

        // Assert
        Assert.Equal(typeOverloadResult, genericResult);
    }

    [Theory]
    [MemberData(nameof(TypesWithTemplates))]
    public void GetId_ReturnsGeneratedId(Type type, string template)
    {
        // Act
        string result = TypeIdentifier.GetId(type);
        string pattern = "^" + Regex.Escape(template).Replace("HASH", "[0-9A-F]{8}") + "$";

        // Assert
        Assert.Matches(pattern, result);
    }

    [Theory]
    [MemberData(nameof(DistinctTypePairs))]
    public void GetId_DifferentTypes_ReturnsDifferentIds(Type first, Type second)
    {
        // Act
        string firstResult = TypeIdentifier.GetId(first);
        string secondResult = TypeIdentifier.GetId(second);

        // Assert
        Assert.NotEqual(firstResult, secondResult);
    }
}
