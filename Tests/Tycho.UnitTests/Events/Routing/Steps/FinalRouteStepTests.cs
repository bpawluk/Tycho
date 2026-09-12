using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;

namespace Tycho.UnitTests.Events.Routing.Steps;

public class FinalRouteStepTests
{
    [Fact]
    public void ToString_ReturnsEndKey()
    {
        // Arrange
        var sut = FinalRouteStep.Create();

        // Act
        string result = sut.ToString();

        // Assert
        Assert.Equal("END", result);
    }

    [Fact]
    public void TryParse_ValidFormat_ReturnsTrueAndFinalStep()
    {
        // Arrange
        string input = "END";

        // Act
        bool success = FinalRouteStep.TryParse(input, out IRouteStep? result);

        // Assert
        Assert.True(success);
        Assert.IsType<FinalRouteStep>(result);
    }

    [Fact]
    public void TryParse_ValidFormatDifferentCasing_ReturnsTrueAndFinalStep()
    {
        // Arrange
        string input = "eNd";

        // Act
        bool success = FinalRouteStep.TryParse(input, out IRouteStep? result);

        // Assert
        Assert.True(success);
        Assert.IsType<FinalRouteStep>(result);
    }

    [Fact]
    public void TryParse_InvalidFormat_ReturnsFalse()
    {
        // Arrange
        string input = "BOGUS";

        // Act
        bool success = FinalRouteStep.TryParse(input, out _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void Parse_ValidFormat_ReturnsFinalStep()
    {
        // Arrange
        string input = "END";

        // Act
        IRouteStep result = FinalRouteStep.Parse(input);

        // Assert
        Assert.IsType<FinalRouteStep>(result);
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        // Arrange
        string input = "BOGUS";

        // Act & Assert
        Assert.Throws<FormatException>(() => FinalRouteStep.Parse(input));
    }
}
