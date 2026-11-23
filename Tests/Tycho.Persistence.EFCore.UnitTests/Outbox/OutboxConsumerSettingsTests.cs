using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.UnitTests.Outbox;

public sealed class OutboxConsumerSettingsTests
{
    [Fact]
    public void Default_ReturnsIndependentSettings()
    {
        // Arrange
        OutboxConsumerSettings first = OutboxConsumerSettings.Default;

        // Act
        OutboxConsumerSettings second = OutboxConsumerSettings.Default;
        first.MaxDeliveryCount = 99;
        first.DeliveryExpiration = TimeSpan.FromHours(1);

        // Assert
        Assert.NotSame(first, second);
        Assert.Equal(3u, second.MaxDeliveryCount);
        Assert.Equal(TimeSpan.FromMinutes(1), second.DeliveryExpiration);
        Assert.Equal(3u, OutboxConsumerSettings.Default.MaxDeliveryCount);
    }

    [Theory]
    [InlineData(true, 0)]
    [InlineData(false, 0)]
    [InlineData(false, -1)]
    public void Registration_RejectsInvalidSettings(bool invalidCount, int seconds)
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        void Act() => services.AddTychoPersistence<TestDbContext>(options =>
        {
            if (invalidCount) options.OutboxConsumer.MaxDeliveryCount = 0;
            else options.OutboxConsumer.DeliveryExpiration = TimeSpan.FromSeconds(seconds);
        });

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(Act);
        Assert.Equal(invalidCount ? "MaxDeliveryCount" : "DeliveryExpiration", exception.ParamName);
        Assert.Empty(services);
    }

    [Fact]
    public void Registration_IsolatesSettingsPerResolution()
    {
        // Arrange
        var services = new ServiceCollection();
        PersistenceSettings? original = null;
        services.AddTychoPersistence<TestDbContext>(options =>
        {
            original = options;
            options.OutboxConsumer.MaxDeliveryCount = 2;
        });
        using ServiceProvider provider = services.BuildServiceProvider();

        // Act
        original!.OutboxConsumer.MaxDeliveryCount = 99;
        OutboxConsumerSettings settings = provider.GetRequiredService<OutboxConsumerSettings>();
        OutboxConsumerSettings next = provider.GetRequiredService<OutboxConsumerSettings>();
        uint registeredCount = settings.MaxDeliveryCount;
        settings.MaxDeliveryCount = 99;
        OutboxConsumerSettings afterCountChange = provider.GetRequiredService<OutboxConsumerSettings>();
        settings.DeliveryExpiration = TimeSpan.Zero;
        OutboxConsumerSettings afterExpirationChange = provider.GetRequiredService<OutboxConsumerSettings>();

        // Assert
        Assert.Equal(2u, registeredCount);
        Assert.NotSame(original.OutboxConsumer, settings);
        Assert.NotSame(settings, next);
        Assert.Equal(2u, afterCountChange.MaxDeliveryCount);
        Assert.Equal(TimeSpan.FromMinutes(1), afterExpirationChange.DeliveryExpiration);
    }

    [Fact]
    public void Validate_AcceptsOneAttempt()
    {
        // Arrange
        var settings = new OutboxConsumerSettings { MaxDeliveryCount = 1 };

        // Act
        Exception? exception = Record.Exception(settings.Validate);

        // Assert
        Assert.Null(exception);
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
