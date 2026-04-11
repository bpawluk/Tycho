using Tycho.Events.Routing.Steps;
using Tycho.Identity.Modules;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Routing.Steps;

public class DownStreamRouteStepTests
{
    [Fact]
    public void Create_SetsDestination()
    {
        // Arrange
        var expectedDestination = ModuleIdentity.Create<TestModule>();

        // Act
        var result = DownStreamRouteStep.Create<TestModule>();

        // Assert
        Assert.Equal(expectedDestination, result.Destination);
    }

    [Fact]
    public void ToString_ReturnsDownKeyWithDestination()
    {
        // Arrange
        var sut = DownStreamRouteStep.Create<TestModule>();
        var expectedDestination = ModuleIdentity.Create<TestModule>();

        // Act
        var result = sut.ToString();

        // Assert
        Assert.Equal($"DOWN({expectedDestination})", result);
    }

    [Fact]
    public void TryParse_ValidFormat_ReturnsTrueAndDownStreamStep()
    {
        // Arrange
        var destination = ModuleIdentity.Create<TestModule>();
        var input = $"DOWN({destination})";

        // Act
        var success = DownStreamRouteStep.TryParse(input, out var result);

        // Assert
        Assert.True(success);
        Assert.IsType<DownStreamRouteStep>(result);
        Assert.Equal(destination, ((DownStreamRouteStep)result).Destination);
    }

    [Fact]
    public void TryParse_ValidFormatDifferentCasing_ReturnsTrueAndDownStreamStep()
    {
        // Arrange
        var destination = ModuleIdentity.Create<TestModule>();
        var input = $"doWn({destination})";

        // Act
        var success = DownStreamRouteStep.TryParse(input, out var result);

        // Assert
        Assert.True(success);
        Assert.IsType<DownStreamRouteStep>(result);
        Assert.Equal(destination, ((DownStreamRouteStep)result).Destination);
    }

    [Fact]
    public void TryParse_InvalidFormat_ReturnsFalse()
    {
        // Arrange
        var input = "BOGUS";

        // Act
        var success = DownStreamRouteStep.TryParse(input, out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void Parse_ValidFormat_ReturnsDownStreamStep()
    {
        // Arrange
        var destination = ModuleIdentity.Create<TestModule>();
        var input = $"DOWN({destination})";

        // Act
        var result = DownStreamRouteStep.Parse(input);

        // Assert
        Assert.IsType<DownStreamRouteStep>(result);
        Assert.Equal(destination, ((DownStreamRouteStep)result).Destination);
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        // Arrange
        var input = "BOGUS";

        // Act & Assert
        Assert.Throws<FormatException>(() => DownStreamRouteStep.Parse(input));
    }
}
