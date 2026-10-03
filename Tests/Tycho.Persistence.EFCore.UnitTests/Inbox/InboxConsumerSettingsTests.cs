using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Persistence.EFCore.Inbox;

namespace Tycho.Persistence.EFCore.UnitTests.Inbox;

public sealed class InboxConsumerSettingsTests
{
    [Fact]
    public void Default_ReturnsIndependentSettings()
    {
        // Arrange
        InboxConsumerSettings first = InboxConsumerSettings.Default;

        // Act
        InboxConsumerSettings second = InboxConsumerSettings.Default;
        first.MaxProcessingCount = 99;
        first.ProcessingExpiration = TimeSpan.FromHours(1);

        // Assert
        Assert.NotSame(first, second);
        Assert.Equal(3u, second.MaxProcessingCount);
        Assert.Equal(TimeSpan.FromMinutes(1), second.ProcessingExpiration);
        Assert.Equal(3u, InboxConsumerSettings.Default.MaxProcessingCount);
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
            if (invalidCount) options.InboxConsumer.MaxProcessingCount = 0;
            else options.InboxConsumer.ProcessingExpiration = TimeSpan.FromSeconds(seconds);
        });

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(Act);
        Assert.Equal(invalidCount ? "MaxProcessingCount" : "ProcessingExpiration", exception.ParamName);
        Assert.Empty(services);
    }

    [Fact]
    public void Registration_IsolatesSettingsPerResolution()
    {
        // Arrange
        var services = new ServiceCollection();
        PersistenceOptions? original = null;
        services.AddTychoPersistence<TestDbContext>(options =>
        {
            original = options;
            options.InboxConsumer.MaxProcessingCount = 2;
        });
        using ServiceProvider provider = services.BuildServiceProvider();

        // Act
        original!.InboxConsumer.MaxProcessingCount = 99;
        InboxConsumerSettings settings = provider.GetRequiredService<InboxConsumerSettings>();
        InboxConsumerSettings next = provider.GetRequiredService<InboxConsumerSettings>();
        uint registeredCount = settings.MaxProcessingCount;
        settings.MaxProcessingCount = 99;
        InboxConsumerSettings afterCountChange = provider.GetRequiredService<InboxConsumerSettings>();
        settings.ProcessingExpiration = TimeSpan.Zero;
        InboxConsumerSettings afterExpirationChange = provider.GetRequiredService<InboxConsumerSettings>();

        // Assert
        Assert.Equal(2u, registeredCount);
        Assert.NotSame(original.InboxConsumer, settings);
        Assert.NotSame(settings, next);
        Assert.Equal(2u, afterCountChange.MaxProcessingCount);
        Assert.Equal(TimeSpan.FromMinutes(1), afterExpirationChange.ProcessingExpiration);
    }

    [Fact]
    public void Validate_AcceptsOneAttempt()
    {
        // Arrange
        var settings = new InboxConsumerSettings { MaxProcessingCount = 1 };

        // Act
        Exception? exception = Record.Exception(settings.Validate);

        // Assert
        Assert.Null(exception);
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
