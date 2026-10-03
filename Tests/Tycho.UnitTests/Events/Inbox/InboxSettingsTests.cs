using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Events.Inbox;

namespace Tycho.UnitTests.Events.Inbox;

public sealed class InboxSettingsTests
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
        var settings = new InboxSettings();
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
            options.Inbox.ConcurrencyLimit = settings.ConcurrencyLimit;
            options.Inbox.InitialPollingInterval = settings.InitialPollingInterval;
            options.Inbox.MaxPollingInterval = settings.MaxPollingInterval;
            options.Inbox.PollingIntervalMultiplier = settings.PollingIntervalMultiplier;
            options.Inbox.MessageProcessingTimeout = settings.MessageProcessingTimeout;
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
        var settings = new InboxSettings { PollingIntervalMultiplier = value };

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
        var settings = new InboxSettings { MessageProcessingTimeout = Timeout.InfiniteTimeSpan };

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
        EventProcessingOptions? original = null;
        services.ConfigureTychoEventProcessing(options =>
        {
            original = options;
            options.Inbox.ConcurrencyLimit = 2;
        });

        // Act
        original!.Inbox.ConcurrencyLimit = 99;
        using ServiceProvider provider = services.BuildServiceProvider();
        InboxSettings settings = provider.GetRequiredService<InboxSettings>();
        InboxSettings next = provider.GetRequiredService<InboxSettings>();
        int registeredCount = settings.ConcurrencyLimit;
        settings.ConcurrencyLimit = 99;
        InboxSettings afterChange = provider.GetRequiredService<InboxSettings>();

        // Assert
        Assert.Equal(2, registeredCount);
        Assert.NotSame(original.Inbox, settings);
        Assert.NotSame(settings, next);
        Assert.Equal(2, afterChange.ConcurrencyLimit);
    }
}
