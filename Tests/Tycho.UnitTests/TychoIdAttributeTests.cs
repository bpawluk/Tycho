namespace Tycho.UnitTests;

public class TychoIdAttributeTests
{
    [Theory]
    [InlineData("A")]
    [InlineData("a")]
    [InlineData("0123456789")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")]
    [InlineData("App.Events-order_1")]
    public void Constructor_WithValidId_PreservesIt(string id)
    {
        // Act
        var sut = new TychoIdAttribute(id);

        // Assert
        Assert.Equal(id, sut.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("order placed")]
    [InlineData("event:1")]
    [InlineData("event/1")]
    [InlineData("event+1")]
    [InlineData("event<1>")]
    [InlineData("événement")]
    [InlineData("事件")]
    [InlineData("event\0")]
    public void Constructor_WithInvalidId_ThrowsArgumentException(string? id)
    {
        // Act
        void Act() => _ = new TychoIdAttribute(id!);

        // Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(Act);
        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithDifferentCase_PreservesDistinctIds()
    {
        // Act
        var first = new TychoIdAttribute("App");
        var second = new TychoIdAttribute("app");

        // Assert
        Assert.Equal("App", first.Id);
        Assert.Equal("app", second.Id);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Theory]
    [InlineData(typeof(AttributedClass), "class")]
    [InlineData(typeof(AttributedStruct), "struct")]
    [InlineData(typeof(IAttributedInterface), "interface")]
    public void Attribute_OnSupportedType_PreservesId(Type type, string expectedId)
    {
        // Act
        var result = (TychoIdAttribute?)Attribute.GetCustomAttribute(type, typeof(TychoIdAttribute), inherit: false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedId, result.Id);
    }

    [TychoId("class")]
    private sealed class AttributedClass { }

    [TychoId("struct")]
    private struct AttributedStruct { }

    [TychoId("interface")]
    private interface IAttributedInterface { }
}
