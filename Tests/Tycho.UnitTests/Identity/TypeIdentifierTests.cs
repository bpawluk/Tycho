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

    public static readonly TheoryData<Type, string> ArrayTypesWithSuffixes = new()
    {
        { typeof(int[]), "[]" },
        { typeof(int).MakeArrayType(1), "[*]" },
        { typeof(int[,]), "[,]" },
        { typeof(int[][]), "[][]" }
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
        string pattern = "^" + Regex.Escape(template).Replace("HASH", "[A-Za-z0-9#&]{11}") + "$";

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

    [Theory]
    [MemberData(nameof(ArrayTypesWithSuffixes))]
    public void GetId_ForArray_PreservesArrayShape(Type arrayType, string suffix)
    {
        // Act
        string result = TypeIdentifier.GetId(arrayType);

        // Assert
        Assert.Equal(TypeIdentifier.GetId<int>() + suffix, result);
    }

    [Theory]
    [InlineData(typeof(StableType), "stable-type")]
    [InlineData(typeof(StableStruct), "stable-struct")]
    [InlineData(typeof(IStableInterface), "stable-interface")]
    [InlineData(typeof(StableGeneric<>), "stable-generic<T>")]
    [InlineData(typeof(StableGeneric<StableType>), "stable-generic<stable-type>")]
    [InlineData(typeof(StableGeneric<StableGeneric<StableType>>), "stable-generic<stable-generic<stable-type>>")]
    [InlineData(typeof(StableType[]), "stable-type[]")]
    [InlineData(typeof(StableType[,]), "stable-type[,]")]
    [InlineData(typeof(StableType[][]), "stable-type[][]")]
    public void GetId_WithExplicitId_UsesItInTheTypeIdentity(Type type, string expectedId)
    {
        // Act
        string result = TypeIdentifier.GetId(type);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public void GetId_WithAttributedBaseType_DoesNotInheritItsId()
    {
        // Act
        string result = TypeIdentifier.GetId<DerivedType>();

        // Assert
        Assert.StartsWith("DerivedType+", result, StringComparison.Ordinal);
        Assert.NotEqual(TypeIdentifier.GetId<StableType>(), result);
    }

    [Fact]
    public void GetId_WithAttributedArgument_UsesItsIdInGeneratedGenericIdentity()
    {
        // Act
        string result = TypeIdentifier.GetId<List<StableType>>();

        // Assert
        Assert.Matches("^List\\+[A-Za-z0-9#&]{11}<stable-type>$", result);
    }

    [Theory]
    [InlineData(typeof(TestModule))]
    [InlineData(typeof(StableType))]
    [InlineData(typeof(StableGeneric<StableType>))]
    public void GetId_WithRepeatedResolution_ReturnsSameId(Type type)
    {
        // Arrange
        string expectedId = TypeIdentifier.GetId(type);

        // Act
        string result = TypeIdentifier.GetId(type);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [TychoId("stable-type")]
    private class StableType { }

    private sealed class DerivedType : StableType { }

    [TychoId("stable-struct")]
    private struct StableStruct { }

    [TychoId("stable-interface")]
    private interface IStableInterface { }

    [TychoId("stable-generic")]
    private sealed class StableGeneric<T> { }
}
