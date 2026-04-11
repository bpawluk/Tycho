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
        var result = sut.ToString();

        // Assert
        Assert.Equal("END", result);
    }

    [Fact]
    public void TryParse_ValidFormat_ReturnsTrueAndFinalStep()
    {
        // Arrange
        var input = "END";

        // Act
        var success = FinalRouteStep.TryParse(input, out var result);

        // Assert
        Assert.True(success);
        Assert.IsType<FinalRouteStep>(result);
    }

    [Fact]
    public void TryParse_ValidFormatDifferentCasing_ReturnsTrueAndFinalStep()
    {
        // Arrange
        var input = "eNd";

        // Act
        var success = FinalRouteStep.TryParse(input, out var result);

        // Assert
        Assert.True(success);
        Assert.IsType<FinalRouteStep>(result);
    }

    [Fact]
    public void TryParse_InvalidFormat_ReturnsFalse()
    {
        // Arrange
        var input = "BOGUS";

        // Act
        var success = FinalRouteStep.TryParse(input, out var result);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void Parse_ValidFormat_ReturnsFinalStep()
    {
        // Arrange
        var input = "END";

        // Act
        var result = FinalRouteStep.Parse(input);

        // Assert
        Assert.IsType<FinalRouteStep>(result);
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        // Arrange
        var input = "BOGUS";

        // Act & Assert
        Assert.Throws<FormatException>(() => FinalRouteStep.Parse(input));
    }
}
