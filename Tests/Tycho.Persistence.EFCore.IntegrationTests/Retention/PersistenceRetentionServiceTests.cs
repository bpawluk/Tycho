using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Retention;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Other;

namespace Tycho.Persistence.EFCore.IntegrationTests.Retention;

public sealed class PersistenceRetentionServiceTests(DatabaseServers servers) : DatabaseTest(servers)
{
    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithEnabledRetention_CleansInboxAndOutbox(DatabaseProvider provider)
    {
        // Arrange
        CancellationToken token = TestContext.Current.CancellationToken;
        await InitializeDatabaseAsync(provider);
        var now = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var owner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
        await using var verification = new TestDbContext(Options<TestDbContext>().Options);
        await verification.Database.EnsureCreatedAsync(token);
        foreach (int age in new[] { 2, 10 })
        {
            verification.Add(new InboxEntry
            {
                OwnerId = owner.Identifier, EntryId = Guid.NewGuid(), State = EntryState.Processed,
                Payload = "original", Created = now.UtcDateTime.AddDays(-age), Updated = now.UtcDateTime.AddDays(-age)
            });
            verification.Add(new OutboxEntry
            {
                OwnerId = owner.Identifier, EntryId = Guid.NewGuid(), State = EntryState.Processed,
                Payload = "original", Created = now.UtcDateTime.AddDays(-age), Updated = now.UtcDateTime.AddDays(-age)
            });
        }
        await verification.SaveChangesAsync(token);
        verification.ChangeTracker.Clear();

        var completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var services = new ServiceCollection();
        services.AddSingleton(owner);
        services.AddScoped<TychoDbContext>(_ => new TestDbContext(Options<TestDbContext>().Options, completed));
        await using ServiceProvider serviceProvider = services.BuildServiceProvider(validateScopes: true);
        var settings = new PersistenceRetentionSettings
        {
            InitialDelay = TimeSpan.Zero,
            Inbox = new() { PayloadRetention = TimeSpan.FromDays(1), FullCleanupRetention = TimeSpan.FromDays(7) },
            Outbox = new() { PayloadRetention = TimeSpan.FromDays(1), FullCleanupRetention = TimeSpan.FromDays(7) }
        };
        using var worker = new PersistenceRetentionService(serviceProvider.GetRequiredService<IServiceScopeFactory>(), settings, new FakeTimeProvider(now));

        // Act
        try
        {
            await worker.StartAsync(token);
            await completed.Task.WaitAsync(TimeSpan.FromSeconds(30), token);
        }
        finally
        {
            await worker.StopAsync(token);
        }

        // Assert
        List<InboxEntry> inbox = await verification.Set<InboxEntry>().AsNoTracking().ToListAsync(token);
        List<OutboxEntry> outbox = await verification.Set<OutboxEntry>().AsNoTracking().ToListAsync(token);
        Assert.Single(inbox);
        Assert.Single(outbox);
        Assert.All(inbox, e => Assert.Equal("{}", e.Payload));
        Assert.All(outbox, e => Assert.Equal("{}", e.Payload));
        }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithDisabledRetention_LeavesInboxAndOutboxUnchanged(DatabaseProvider provider)
    {
        // Arrange
        CancellationToken token = TestContext.Current.CancellationToken;
        await InitializeDatabaseAsync(provider);
        var now = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var owner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
        await using var verification = new TestDbContext(Options<TestDbContext>().Options);
        await verification.Database.EnsureCreatedAsync(token);
        foreach (int age in new[] { 2, 10 })
        {
            verification.Add(new InboxEntry
            {
                OwnerId = owner.Identifier, EntryId = Guid.NewGuid(), State = EntryState.Processed,
                Payload = "original", Created = now.UtcDateTime.AddDays(-age), Updated = now.UtcDateTime.AddDays(-age)
            });
            verification.Add(new OutboxEntry
            {
                OwnerId = owner.Identifier, EntryId = Guid.NewGuid(), State = EntryState.Processed,
                Payload = "original", Created = now.UtcDateTime.AddDays(-age), Updated = now.UtcDateTime.AddDays(-age)
            });
        }
        await verification.SaveChangesAsync(token);
        verification.ChangeTracker.Clear();

        var completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var services = new ServiceCollection();
        services.AddSingleton(owner);
        services.AddScoped<TychoDbContext>(_ => new TestDbContext(Options<TestDbContext>().Options, completed));
        await using ServiceProvider serviceProvider = services.BuildServiceProvider(validateScopes: true);
        var settings = new PersistenceRetentionSettings
        {
            InitialDelay = TimeSpan.Zero,
            Inbox = new() { PayloadRetention = null, FullCleanupRetention = null },
            Outbox = new() { PayloadRetention = null, FullCleanupRetention = null }
        };
        using var worker = new PersistenceRetentionService(serviceProvider.GetRequiredService<IServiceScopeFactory>(), settings, new FakeTimeProvider(now));

        // Act
        try
        {
            await worker.StartAsync(token);
            await completed.Task.WaitAsync(TimeSpan.FromSeconds(30), token);
        }
        finally
        {
            await worker.StopAsync(token);
        }

        // Assert
        List<InboxEntry> inbox = await verification.Set<InboxEntry>().AsNoTracking().ToListAsync(token);
        List<OutboxEntry> outbox = await verification.Set<OutboxEntry>().AsNoTracking().ToListAsync(token);
        Assert.Equal(2, inbox.Count);
        Assert.Equal(2, outbox.Count);
        Assert.All(inbox, e => Assert.Equal("original", e.Payload));
        Assert.All(outbox, e => Assert.Equal("original", e.Payload));
        }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options, TaskCompletionSource? completed = null) : TychoDbContext(options)
    {
        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            completed?.TrySetResult();
        }
    }
}
