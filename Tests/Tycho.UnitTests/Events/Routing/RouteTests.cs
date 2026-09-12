using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;
using Tycho.Identity.Modules;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Routing;

public class RouteTests
{
    [Fact]
    public void Create_ReturnsRouteWithFinalStep()
    {
        // Act
        var result = Route.Create();

        // Assert
        IRouteStep step = Assert.Single(result);
        Assert.IsType<FinalRouteStep>(step);
    }

    [Fact]
    public void ToString_WithEmptyRoute_ReturnsEnd()
    {
        // Arrange
        var sut = Route.Create();

        // Act
        string result = sut.ToString();

        // Assert
        Assert.Equal("END", result);
    }

    [Fact]
    public void ToString_WithMultipleSteps_ReturnsSeparatedString()
    {
        // Arrange
        var sut = Route.Create();
        sut.Push(DownStreamRouteStep.Create<TestModule>());
        sut.Push(UpStreamRouteStep.Create());

        var expectedDestination = ModuleIdentity.Create<TestModule>();

        // Act
        string result = sut.ToString();

        // Assert
        Assert.Equal($"UP/DOWN({expectedDestination})/END", result);
    }

    [Fact]
    public void Parse_FinalStepOnly_ReturnsRouteWithFinalStep()
    {
        // Arrange
        string input = "END";

        // Act
        var result = Route.Parse(input);

        // Assert
        IRouteStep step = Assert.Single(result);
        Assert.IsType<FinalRouteStep>(step);
    }

    [Fact]
    public void Parse_WithDownStreamStep_ReturnsCorrectRoute()
    {
        // Arrange
        var destination = ModuleIdentity.Create<TestModule>();
        string input = $"DOWN({destination})/END";

        // Act
        var result = Route.Parse(input);

        // Assert
        Assert.Equal(2, result.Count);
        IRouteStep nextStep = result.Pop();
        Assert.IsType<DownStreamRouteStep>(nextStep);
        Assert.Equal(destination, ((DownStreamRouteStep)nextStep).Destination);
    }

    [Fact]
    public void Parse_WithUpStreamStep_ReturnsCorrectRoute()
    {
        // Arrange
        string input = "UP/END";

        // Act
        var result = Route.Parse(input);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.IsType<UpStreamRouteStep>(result.Peek());
    }

    [Fact]
    public void Parse_ComplexRoute_ReturnsCorrectRoute()
    {
        // Arrange
        var destination = ModuleIdentity.Create<TestModule>();
        string input = $"UP/DOWN({destination})/END";

        // Act
        var result = Route.Parse(input);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.IsType<UpStreamRouteStep>(result.Pop());
        Assert.IsType<DownStreamRouteStep>(result.Pop());
        Assert.IsType<FinalRouteStep>(result.Pop());
    }

    [Fact]
    public void Parse_InvalidStep_ThrowsFormatException()
    {
        // Arrange
        string input = "UP/BOGUS/END";

        // Act
        void Act() => Route.Parse(input);

        // Assert
        Assert.Throws<FormatException>(Act);
    }
}
