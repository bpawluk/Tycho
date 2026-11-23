using Microsoft.EntityFrameworkCore;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Identity.Structure;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.TestUtils.Other;

namespace Tycho.Persistence.EFCore.IntegrationTests.Common.Outbox;

public abstract class OutboxConsumerTests : IntegrationTestsBase
{
    private readonly OutboxConsumerSettings _settings = new()
    {
        MaxDeliveryCount = 3,
        DeliveryExpiration = TimeSpan.FromMinutes(5)
    };

    private readonly TestDbContext _dbContext;

    private readonly OutboxConsumer _sut;

    protected OutboxConsumerTests(IntegrationTestsDatabaseBase database) : base(database)
    {
        DbContextOptions<TestDbContext> options = Database.CreateOptionsBuilder<TestDbContext>().Options;

        _dbContext = new TestDbContext(options);

        var persistenceOwner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
        _sut = new OutboxConsumer(_dbContext, persistenceOwner, _settings);
    }

    [Fact(Timeout = 60000)]
    public async Task TryReadAsync_WithNewEntry_ReturnsMappedEvent()
    {
        // Arrange
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

    [Fact(Timeout = 60000)]
    public async Task MarkAsDeliveredAsync_WithReadEvent_UpdatesEntryStatus()
    {
        // Arrange
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

    [Fact(Timeout = 60000)]
    public async Task MarkAsFailedAsync_WithReadEvent_UpdatesEntryStatus()
    {
        // Arrange
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

    [Fact(Timeout = 60000)]
    public async Task TryReadAsync_WithEmptyQueue_ReturnsNull()
    {
        // Act
        OutboxEvent? result = await _sut.TryReadAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact(Timeout = 60000)]
    public async Task TryReadAsync_WhenFinalClaimHasInvalidDestination_MarksEntryAsFailed()
    {
        // Arrange
        _settings.MaxDeliveryCount = 1;
        OutboxEntry entry = CreateEntry(Guid.NewGuid(), EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        entry.Destination = " ";
        await SeedEntries(entry);

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.TryReadAsync(TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(EntryState.Failed, (await LoadEntry(entry.EntryId)).State);
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
        await _dbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
