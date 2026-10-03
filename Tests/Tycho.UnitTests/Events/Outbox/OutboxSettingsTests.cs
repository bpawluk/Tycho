using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Events.Outbox;

namespace Tycho.UnitTests.Events.Outbox;

public sealed class OutboxSettingsTests
{
    [Theory]
    [InlineData("ConcurrencyLimit")]
    [InlineData("InitialPollingInterval")]
    [InlineData("MaxPollingInterval")]
    [InlineData("PollingIntervalMultiplier")]
    [InlineData("MessageProcessingTimeout")]
    public void InvalidSettings_AreRejectedUponRegistration(string property)
    {
        // Arrange
        var settings = new OutboxSettings();
        switch (property)
        {
            case "ConcurrencyLimit": settings.ConcurrencyLimit = 0; break;
            case "InitialPollingInterval": settings.InitialPollingInterval = TimeSpan.Zero; break;
            case "MaxPollingInterval": settings.MaxPollingInterval = TimeSpan.Zero; break;
            case "PollingIntervalMultiplier": settings.PollingIntervalMultiplier = double.NaN; break;
            case "MessageProcessingTimeout": settings.MessageProcessingTimeout = TimeSpan.Zero; break;
        }
        var services = new ServiceCollection();
        services.ConfigureTychoEventProcessing(_ => { });
        ServiceDescriptor[] original = [.. services];

        // Act
        void Act() => services.ConfigureTychoEventProcessing(options =>
        {
            options.Outbox.ConcurrencyLimit = settings.ConcurrencyLimit;
            options.Outbox.InitialPollingInterval = settings.InitialPollingInterval;
            options.Outbox.MaxPollingInterval = settings.MaxPollingInterval;
            options.Outbox.PollingIntervalMultiplier = settings.PollingIntervalMultiplier;
            options.Outbox.MessageProcessingTimeout = settings.MessageProcessingTimeout;
        });

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(Act);
        Assert.Equal(property, exception.ParamName);
        Assert.Equal(original, [.. services]);
    }

    [Theory]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(1.0)]
    public void Validate_RejectsInvalidMultiplier(double value)
    {
        // Arrange
        var settings = new OutboxSettings { PollingIntervalMultiplier = value };

        // Act
        void Act() => settings.Validate();

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(Act);
        Assert.Equal("PollingIntervalMultiplier", exception.ParamName);
    }

    [Fact]
    public void Validate_AcceptsInfiniteTimeout()
    {
        // Arrange
        var settings = new OutboxSettings { MessageProcessingTimeout = Timeout.InfiniteTimeSpan };

        // Act
        Exception? exception = Record.Exception(settings.Validate);

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Registration_IsolatesSettingsPerResolution()
    {
        // Arrange
        var services = new ServiceCollection();
        EventProcessingSettings? original = null;
        services.ConfigureTychoEventProcessing(options =>
        {
            original = options;
            options.Outbox.ConcurrencyLimit = 2;
        });

        // Act
        original!.Outbox.ConcurrencyLimit = 99;
        using ServiceProvider provider = services.BuildServiceProvider();
        OutboxSettings settings = provider.GetRequiredService<OutboxSettings>();
        OutboxSettings next = provider.GetRequiredService<OutboxSettings>();
        int registeredCount = settings.ConcurrencyLimit;
        settings.ConcurrencyLimit = 99;
        OutboxSettings afterChange = provider.GetRequiredService<OutboxSettings>();

        // Assert
        Assert.Equal(2, registeredCount);
        Assert.NotSame(original.Outbox, settings);
        Assert.NotSame(settings, next);
        Assert.Equal(2, afterChange.ConcurrencyLimit);
    }
}
