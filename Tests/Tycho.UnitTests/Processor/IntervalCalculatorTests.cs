using Tycho.Processor;

namespace Tycho.UnitTests.Processor;

public sealed class IntervalCalculatorTests
{
    [Fact]
    public void Constructor_SetsCurrentToInitialInterval()
    {
        // Arrange
        TimeSpan initial = TimeSpan.FromTicks(10);

        // Act
        var sut = new IntervalCalculator(initial, TimeSpan.FromTicks(100), 2);

        // Assert
        Assert.Equal(initial, sut.Current);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenInitialIsNotPositive_ThrowsForInitial(long initialTicks)
    {
        // Arrange
        TimeSpan initial = TimeSpan.FromTicks(initialTicks);

        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new IntervalCalculator(initial, TimeSpan.FromTicks(100), 2));

        // Assert
        Assert.Equal("initial", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenMaximumIsBelowInitial_ThrowsForMaximum()
    {
        // Arrange
        TimeSpan initial = TimeSpan.FromTicks(10);
        TimeSpan maximal = TimeSpan.FromTicks(9);

        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new IntervalCalculator(initial, maximal, 2));

        // Assert
        Assert.Equal("maximal", exception.ParamName);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_WhenMultiplierIsInvalid_ThrowsForMultiplier(double multiplier)
    {
        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new IntervalCalculator(TimeSpan.FromTicks(10), TimeSpan.FromTicks(100), multiplier));

        // Assert
        Assert.Equal("multiplier", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenMaximumEqualsInitial_AcceptsInterval()
    {
        // Arrange
        TimeSpan interval = TimeSpan.FromTicks(10);

        // Act
        var sut = new IntervalCalculator(interval, interval, 2);

        // Assert
        Assert.Equal(interval, sut.Current);
    }

    [Fact]
    public void Increase_WhenBelowMaximum_MultipliesCurrentTicks()
    {
        // Arrange
        var sut = new IntervalCalculator(TimeSpan.FromTicks(10), TimeSpan.FromTicks(100), 2.5);

        // Act
        sut.Increase();

        // Assert
        Assert.Equal(TimeSpan.FromTicks(25), sut.Current);
    }

    [Fact]
    public void Increase_WhenProductHasFractionalTicks_TruncatesFraction()
    {
        // Arrange
        var sut = new IntervalCalculator(TimeSpan.FromTicks(3), TimeSpan.FromTicks(100), 1.5);

        // Act
        sut.Increase();

        // Assert
        Assert.Equal(TimeSpan.FromTicks(4), sut.Current);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(15)]
    public void Increase_WhenProductReachesOrExceedsMaximum_ClampsToMaximum(long maximalTicks)
    {
        // Arrange
        TimeSpan maximal = TimeSpan.FromTicks(maximalTicks);
        var sut = new IntervalCalculator(TimeSpan.FromTicks(10), maximal, 2);

        // Act
        sut.Increase();

        // Assert
        Assert.Equal(maximal, sut.Current);
    }

    [Fact]
    public void Increase_WhenAlreadyAtMaximum_RemainsAtMaximum()
    {
        // Arrange
        TimeSpan maximal = TimeSpan.FromTicks(10);
        var sut = new IntervalCalculator(maximal, maximal, 2);

        // Act
        sut.Increase();

        // Assert
        Assert.Equal(maximal, sut.Current);
    }

    [Fact]
    public void Reset_AfterIncreases_RestoresInitialInterval()
    {
        // Arrange
        TimeSpan initial = TimeSpan.FromTicks(10);
        var sut = new IntervalCalculator(initial, TimeSpan.FromTicks(100), 2);
        sut.Increase();
        sut.Increase();

        // Act
        sut.Reset();

        // Assert
        Assert.Equal(initial, sut.Current);
    }

    [Fact]
    public void Increase_AfterReset_StartsFromInitialInterval()
    {
        // Arrange
        var sut = new IntervalCalculator(TimeSpan.FromTicks(10), TimeSpan.FromTicks(100), 2);
        sut.Increase();
        sut.Increase();
        sut.Reset();

        // Act
        sut.Increase();

        // Assert
        Assert.Equal(TimeSpan.FromTicks(20), sut.Current);
    }
}
