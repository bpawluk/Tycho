using Tycho.Events.Routing;
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
        string result = sut.ToString();

        // Assert
        Assert.Equal("UP", result);
    }

    [Fact]
    public void TryParse_ValidFormat_ReturnsTrueAndUpStreamStep()
    {
        // Arrange
        string input = "UP";

        // Act
        bool success = UpStreamRouteStep.TryParse(input, out IRouteStep? result);

        // Assert
        Assert.True(success);
        Assert.IsType<UpStreamRouteStep>(result);
    }

    [Fact]
    public void TryParse_ValidFormatDifferentCasing_ReturnsTrueAndUpStreamStep()
    {
        // Arrange
        string input = "uP";

        // Act
        bool success = UpStreamRouteStep.TryParse(input, out IRouteStep? result);

        // Assert
        Assert.True(success);
        Assert.IsType<UpStreamRouteStep>(result);
    }

    [Fact]
    public void TryParse_InvalidFormat_ReturnsFalse()
    {
        // Arrange
        string input = "BOGUS";

        // Act
        bool success = UpStreamRouteStep.TryParse(input, out _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void Parse_ValidFormat_ReturnsUpStreamStep()
    {
        // Arrange
        string input = "UP";

        // Act
        IRouteStep result = UpStreamRouteStep.Parse(input);

        // Assert
        Assert.IsType<UpStreamRouteStep>(result);
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        // Arrange
        string input = "BOGUS";

        // Act & Assert
        Assert.Throws<FormatException>(() => UpStreamRouteStep.Parse(input));
    }
}
