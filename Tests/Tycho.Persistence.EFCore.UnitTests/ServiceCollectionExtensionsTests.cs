using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Events.Inbox;
using Tycho.Events.Outbox;
using Tycho.Persistence.EFCore.Common;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Retention;
using Tycho.Persistence.EFCore.Transactions;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UnitTests;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTychoPersistence_RegistersPersistenceServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        IServiceCollection result = services.AddTychoPersistence<TestDbContext>();

        // Assert
        Assert.Same(services, result);
        AssertRegistration<PersistenceOwner, PersistenceOwner>(services, ServiceLifetime.Singleton);
        AssertRegistration<ITransaction, Transaction>(services, ServiceLifetime.Scoped);
        AssertRegistration<IInboxWriter, InboxWriter>(services, ServiceLifetime.Transient);
        AssertRegistration<IInboxConsumer, InboxConsumer>(services, ServiceLifetime.Transient);
        AssertRegistration<IOutboxWriter, OutboxWriter>(services, ServiceLifetime.Transient);
        AssertRegistration<IOutboxConsumer, OutboxConsumer>(services, ServiceLifetime.Transient);
    }

    [Fact]
    public void AddTychoPersistence_SharesContextWithinScope()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTychoPersistence<TestDbContext>();
        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        using IServiceScope firstScope = provider.CreateScope();
        using IServiceScope secondScope = provider.CreateScope();

        // Act
        TestDbContext firstContext = firstScope.ServiceProvider.GetRequiredService<TestDbContext>();
        TychoDbContext firstBaseContext = firstScope.ServiceProvider.GetRequiredService<TychoDbContext>();
        TestDbContext secondContext = secondScope.ServiceProvider.GetRequiredService<TestDbContext>();
        TychoDbContext secondBaseContext = secondScope.ServiceProvider.GetRequiredService<TychoDbContext>();

        // Assert
        Assert.Same(firstContext, firstBaseContext);
        Assert.Same(secondContext, secondBaseContext);
        Assert.NotSame(firstContext, secondContext);
    }

    [Fact]
    public void AddTychoPersistence_WithNullServices_Throws()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act
        void Act() => services.AddTychoPersistence<TestDbContext>();

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(Act);
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddTychoPersistenceRetention_RegistersDefaultRetentionServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        IServiceCollection result = services.AddTychoPersistenceRetention();

        // Assert
        Assert.Same(services, result);
        ServiceDescriptor registration = Assert.Single(services, service => service.ServiceType == typeof(PersistenceRetentionOptions));
        Assert.Equal(ServiceLifetime.Singleton, registration.Lifetime);
        PersistenceRetentionOptions options = Assert.IsType<PersistenceRetentionOptions>(registration.ImplementationInstance);
        Assert.True(options.IsEnabled);
        AssertRegistration<IInboxCleaner, InboxCleaner>(services, ServiceLifetime.Scoped);
        AssertRegistration<IOutboxCleaner, OutboxCleaner>(services, ServiceLifetime.Scoped);
        AssertRegistration<IHostedService, PersistenceRetentionService>(services, ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddTychoPersistenceRetention_WithConfiguration_UsesConfiguredOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTychoPersistenceRetention(options =>
        {
            options.CleanupInterval = TimeSpan.FromMinutes(15);
            options.Inbox.PayloadRetention = TimeSpan.FromDays(2);
            options.Outbox.FullCleanupRetention = TimeSpan.FromDays(3);
        });

        // Assert
        using ServiceProvider provider = services.BuildServiceProvider();
        PersistenceRetentionOptions options = provider.GetRequiredService<PersistenceRetentionOptions>();
        Assert.Equal(TimeSpan.FromMinutes(15), options.CleanupInterval);
        Assert.Equal(TimeSpan.FromDays(2), options.Inbox.PayloadRetention);
        Assert.Equal(TimeSpan.FromDays(3), options.Outbox.FullCleanupRetention);
    }

    [Fact]
    public void AddTychoPersistenceRetention_WithInvalidConfiguration_LeavesRegistrationsUnchanged()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTychoPersistenceRetention();
        ServiceDescriptor[] originalRegistrations = [.. services];

        // Act
        void Act() => services.AddTychoPersistenceRetention(options => options.CleanupInterval = TimeSpan.Zero);

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(Act);
        Assert.Equal(nameof(PersistenceRetentionOptions.CleanupInterval), exception.ParamName);
        Assert.Equal(originalRegistrations, [.. services]);
    }

    [Fact]
    public void AddTychoPersistenceRetention_WhenDisabled_DoesNotRegisterWorker()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTychoPersistenceRetention(options =>
        {
            options.Inbox.PayloadRetention = null;
            options.Outbox.FullCleanupRetention = null;
        });

        // Assert
        Assert.DoesNotContain(services, service => service.ServiceType == typeof(IHostedService));
    }

    [Fact]
    public void AddTychoPersistenceRetention_WhenCalledAgain_ReplacesOptionsWithoutDuplicatingWorker()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTychoPersistenceRetention(options => options.CleanupInterval = TimeSpan.FromMinutes(30));

        // Act
        services.AddTychoPersistenceRetention(options => options.CleanupInterval = TimeSpan.FromMinutes(10));

        // Assert
        ServiceDescriptor registration = Assert.Single(services, service => service.ServiceType == typeof(PersistenceRetentionOptions));
        PersistenceRetentionOptions options = Assert.IsType<PersistenceRetentionOptions>(registration.ImplementationInstance);
        Assert.Equal(TimeSpan.FromMinutes(10), options.CleanupInterval);
        AssertRegistration<IHostedService, PersistenceRetentionService>(services, ServiceLifetime.Singleton);
        AssertRegistration<IInboxCleaner, InboxCleaner>(services, ServiceLifetime.Scoped);
        AssertRegistration<IOutboxCleaner, OutboxCleaner>(services, ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddTychoPersistenceRetention_WhenDisabledAfterRegistration_RemovesOnlyRetentionWorker()
    {
        // Arrange
        var services = new ServiceCollection();
        IHostedService otherWorker = Mock.Of<IHostedService>();
        services.AddSingleton(otherWorker);
        services.AddTychoPersistenceRetention();

        // Act
        services.AddTychoPersistenceRetention(options =>
        {
            options.Inbox.PayloadRetention = null;
            options.Outbox.FullCleanupRetention = null;
        });

        // Assert
        ServiceDescriptor registration = Assert.Single(services, service => service.ServiceType == typeof(IHostedService));
        Assert.Same(otherWorker, registration.ImplementationInstance);
    }

    [Fact]
    public void AddTychoPersistenceRetention_WithExistingCleaners_PreservesRegistrations()
    {
        // Arrange
        var services = new ServiceCollection();
        IInboxCleaner inboxCleaner = Mock.Of<IInboxCleaner>();
        IOutboxCleaner outboxCleaner = Mock.Of<IOutboxCleaner>();
        services.AddSingleton(inboxCleaner);
        services.AddSingleton(outboxCleaner);

        // Act
        services.AddTychoPersistenceRetention();

        // Assert
        ServiceDescriptor inboxRegistration = Assert.Single(services, service => service.ServiceType == typeof(IInboxCleaner));
        ServiceDescriptor outboxRegistration = Assert.Single(services, service => service.ServiceType == typeof(IOutboxCleaner));
        Assert.Same(inboxCleaner, inboxRegistration.ImplementationInstance);
        Assert.Same(outboxCleaner, outboxRegistration.ImplementationInstance);
    }

    [Fact]
    public void AddTychoPersistenceRetention_WithNullServices_Throws()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act
        void Act() => services.AddTychoPersistenceRetention();

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(Act);
        Assert.Equal("services", exception.ParamName);
    }

    private static void AssertRegistration<TService, TImplementation>(IServiceCollection services, ServiceLifetime lifetime)
    {
        ServiceDescriptor registration = Assert.Single(services, service => service.ServiceType == typeof(TService));
        Assert.Equal(typeof(TImplementation), registration.ImplementationType);
        Assert.Equal(lifetime, registration.Lifetime);
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
