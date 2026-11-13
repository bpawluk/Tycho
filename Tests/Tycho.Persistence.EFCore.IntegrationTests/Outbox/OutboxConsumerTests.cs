using Microsoft.EntityFrameworkCore;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Identity.Structure;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Other;

namespace Tycho.Persistence.EFCore.IntegrationTests.Outbox;

public sealed class OutboxConsumerTests(DatabaseServers servers) : DatabaseTest(servers)
{
    private readonly OutboxConsumerSettings _settings = new()
    {
        MaxDeliveryCount = 3,
        DeliveryExpiration = TimeSpan.FromMinutes(5)
    };

    private TestDbContext _dbContext = default!;

    private OutboxConsumer _sut = default!;

    private async Task SetupAsync(DatabaseProvider provider)
    {
        await InitializeDatabaseAsync(provider);

        DbContextOptions<TestDbContext> options = Options<TestDbContext>().Options;

        _dbContext = new TestDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        var persistenceOwner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
        _sut = new OutboxConsumer(_dbContext, persistenceOwner, _settings);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task TryReadAsync_WithNewEntry_ReturnsMappedEvent(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry(Guid.NewGuid(), EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        entry.Handler = "test-handler";
        entry.Payload = "{\"value\":1}";
        await SeedEntries(entry);

        // Act
        OutboxEvent? result = await _sut.TryReadAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.ClaimId);
        AssertRoutedEvent(entry, result.RoutedEvent);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task MarkAsDeliveredAsync_WithReadEvent_UpdatesEntryStatus(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry(Guid.NewGuid(), EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        await SeedEntries(entry);
        OutboxEvent? result = await _sut.TryReadAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(result);

        // Act
        bool updated = await _sut.MarkAsDeliveredAsync(result.EventId, result.ClaimId, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(updated);
        Assert.Equal(EntryState.Processed, (await LoadEntry(entry.EntryId)).State);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task MarkAsFailedAsync_WithReadEvent_UpdatesEntryStatus(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry(Guid.NewGuid(), EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        await SeedEntries(entry);
        OutboxEvent? result = await _sut.TryReadAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(result);

        // Act
        bool updated = await _sut.MarkAsFailedAsync(result.EventId, result.ClaimId, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(updated);
        Assert.Equal(EntryState.Failed, (await LoadEntry(entry.EntryId)).State);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task TryReadAsync_WithEmptyQueue_ReturnsNull(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);

        // Act
        OutboxEvent? result = await _sut.TryReadAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    private async Task SeedEntries(params OutboxEntry[] entries)
    {
        _dbContext.Set<OutboxEntry>().AddRange(entries);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<OutboxEntry> LoadEntry(Guid id)
    {
        _dbContext.ChangeTracker.Clear();

        OutboxEntry? entry = await _dbContext
            .Set<OutboxEntry>()
            .AsNoTracking()
            .SingleOrDefaultAsync(outboxEntry => outboxEntry.EntryId == id);

        Assert.NotNull(entry);
        return entry!;
    }

    private static void AssertRoutedEvent(OutboxEntry expected, SerializedRoutedEvent actual)
    {
        Assert.Equal(expected.EntryId, actual.Id);
        Assert.Equal(expected.PublishId, actual.PublishId);
        Assert.Equal(expected.Event, actual.EventId.Value);
        Assert.Equal(expected.Handler, actual.HandlerId.Value);
        Assert.Equal(expected.Destination, actual.DestinationId.Value);
        Assert.Equal(expected.Payload, actual.Payload);
    }

    private static OutboxEntry CreateEntry(
        Guid id,
        EntryState state,
        uint deliveryAttempts,
        Guid claimId,
        DateTime claimExpiration)
    {
        DateTime now = DateTime.UtcNow;
        var persistenceOwner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
        return new OutboxEntry
        {
            OwnerId = persistenceOwner.Identifier,
            EntryId = id,
            PublishId = Guid.NewGuid(),
            Destination = InstanceIdentity.Parse("test-endpoint").ToString(),
            Handler = "TestHandler",
            Event = "TestEvent",
            Payload = "{}",
            State = state,
            Created = now.AddMinutes(-1),
            Updated = now,
            Attempts = deliveryAttempts,
            ClaimId = claimId,
            ClaimExpiration = claimExpiration
        };
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            if (_dbContext is not null) await _dbContext.DisposeAsync();
        }
        finally
        {
            await base.DisposeAsync();
        }
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
