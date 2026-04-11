using Tycho.Events.Routing.Steps;

namespace Tycho.UnitTests.Events.Routing.Steps;

public class UpStreamRouteStepTests
{
    [Fact]
    public void ToString_ReturnsUpKey()
    {
        // Arrange
        var sut = UpStreamRouteStep.Create();

        // Act
        var result = sut.ToString();

        // Assert
        Assert.Equal("UP", result);
    }

    [Fact]
    public void TryParse_ValidFormat_ReturnsTrueAndUpStreamStep()
    {
        // Arrange
        var input = "UP";

        // Act
        var success = UpStreamRouteStep.TryParse(input, out var result);

        // Assert
        Assert.True(success);
        Assert.IsType<UpStreamRouteStep>(result);
    }

    [Fact]
    public void TryParse_ValidFormatDifferentCasing_ReturnsTrueAndUpStreamStep()
    {
        // Arrange
        var input = "uP";

        // Act
        var success = UpStreamRouteStep.TryParse(input, out var result);

        // Assert
        Assert.True(success);
        Assert.IsType<UpStreamRouteStep>(result);
    }

    [Fact]
    public void TryParse_InvalidFormat_ReturnsFalse()
    {
        // Arrange
        var input = "BOGUS";

        // Act
        var success = UpStreamRouteStep.TryParse(input, out var result);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void Parse_ValidFormat_ReturnsUpStreamStep()
    {
        // Arrange
        var input = "UP";

        // Act
        var result = UpStreamRouteStep.Parse(input);

        // Assert
        Assert.IsType<UpStreamRouteStep>(result);
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        // Arrange
        var input = "BOGUS";

        // Act & Assert
        Assert.Throws<FormatException>(() => UpStreamRouteStep.Parse(input));
    }
}
