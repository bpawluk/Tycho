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
    public void Increase_WhenProductHasFractionalTicks_RoundsUp()
    {
        // Arrange
        var sut = new IntervalCalculator(TimeSpan.FromTicks(3), TimeSpan.FromTicks(100), 1.5);

        // Act
        sut.Increase();

        // Assert
        Assert.Equal(TimeSpan.FromTicks(5), sut.Current);
    }

    [Fact]
    public void Increase_WhenMultiplierIsCloseToOne_AdvancesUntilMaximum()
    {
        // Arrange
        TimeSpan initial = TimeSpan.FromMilliseconds(1);
        TimeSpan next = initial + TimeSpan.FromTicks(1);
        TimeSpan maximal = initial + TimeSpan.FromTicks(2);
        var sut = new IntervalCalculator(initial, maximal, Math.BitIncrement(1.0));

        // Act
        sut.Increase();
        TimeSpan first = sut.Current;

        sut.Increase();
        TimeSpan second = sut.Current;

        sut.Increase();
        TimeSpan third = sut.Current;

        // Assert
        Assert.Equal(next, first);
        Assert.Equal(maximal, second);
        Assert.Equal(maximal, third);
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
