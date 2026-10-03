using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.UnitTests;

public sealed class PersistenceOptionsTests
{
    [Fact]
    public void AddPersistence_WithoutConfigurationReplacesExistingSettingsWithDefaults()
    {
        // Arrange
        var services = new ServiceCollection();
        var inbox = new InboxConsumerSettings { MaxProcessingCount = 8 };
        services.AddSingleton(inbox);

        // Act
        services.AddTychoPersistence<TestDbContext>();
        using ServiceProvider provider = services.BuildServiceProvider();
        InboxConsumerSettings[] inboxSettings = [.. provider.GetServices<InboxConsumerSettings>()];
        OutboxConsumerSettings[] outboxSettings = [.. provider.GetServices<OutboxConsumerSettings>()];

        // Assert
        InboxConsumerSettings registeredInbox = Assert.Single(inboxSettings);
        Assert.NotSame(inbox, registeredInbox);
        Assert.Equal(3u, registeredInbox.MaxProcessingCount);
        Assert.Equal(3u, Assert.Single(outboxSettings).MaxDeliveryCount);
    }

    [Fact]
    public void AddPersistence_ConfiguresAndRegistersSeparateConsumerSettings()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTychoPersistence<TestDbContext>();

        // Act
        IServiceCollection result = services.AddTychoPersistence<TestDbContext>(options =>
        {
            options.InboxConsumer.MaxProcessingCount = 5;
            options.OutboxConsumer.MaxDeliveryCount = 7;
        });
        using ServiceProvider provider = services.BuildServiceProvider();

        // Assert
        Assert.Same(services, result);
        Assert.Equal(5u, Assert.Single(provider.GetServices<InboxConsumerSettings>()).MaxProcessingCount);
        Assert.Equal(7u, Assert.Single(provider.GetServices<OutboxConsumerSettings>()).MaxDeliveryCount);
        Assert.Null(provider.GetService<PersistenceOptions>());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddPersistence_InvalidSettingsLeavesAllRegistrationsUnchanged(bool inbox)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTychoPersistence<TestDbContext>();
        ServiceDescriptor[] original = [.. services];

        // Act
        void Act() => services.AddTychoPersistence<TestDbContext>(options =>
        {
            if (inbox) options.InboxConsumer.MaxProcessingCount = 0;
            else options.OutboxConsumer.DeliveryExpiration = TimeSpan.Zero;
        });

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
        Assert.Equal(original, [.. services]);
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
