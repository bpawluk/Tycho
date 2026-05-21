using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Persistence.EFCore.Common;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.UnitTests.Outbox;

public sealed class OutboxConsumerTests : IAsyncLifetime
{
    private readonly OutboxConsumerSettings _settings = new()
    {
        MaxDeliveryCount = 3,
        DeliveryExpiration = TimeSpan.FromMinutes(5)
    };

    private TestDbContext _dbContext = default!;
    private SqliteConnection _connection = default!;

    private OutboxConsumer _sut = default!;

    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        DbContextOptionsBuilder<TestDbContext> optionsBuilder = new();
        DbContextOptions<TestDbContext> options = optionsBuilder.UseSqlite(_connection).Options;

        _dbContext = new TestDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        _sut = new OutboxConsumer(_dbContext, _settings);
    }

    [Fact]
    public async Task Read_WithNewEntries_ClaimsAndReadsTheEntries()
    {
        // Arrange
        Guid firstEntryId = Guid.NewGuid();
        Guid secondEntryId = Guid.NewGuid();
        OutboxEntry firstEntry = CreateEntry(firstEntryId, EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        OutboxEntry secondEntry = CreateEntry(secondEntryId, EntryState.New, 0, Guid.Empty, DateTime.MinValue);

        await SeedEntries(firstEntry, secondEntry);

        DateTime readStartedAt = DateTime.UtcNow;

        // Act
        IReadOnlyCollection<OutboxEvent> result = await _sut.Read(2, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Guid[] returnedIds = [.. result.Select(outboxEvent => outboxEvent.EventId)];
        Assert.Contains(firstEntryId, returnedIds);
        Assert.Contains(secondEntryId, returnedIds);

        Guid claimId = Assert.Single(result.Select(outboxEvent => outboxEvent.ClaimId).Distinct());
        Assert.NotEqual(Guid.Empty, claimId);

        OutboxEntry persistedFirstEntry = await LoadEntry(firstEntryId);
        OutboxEntry persistedSecondEntry = await LoadEntry(secondEntryId);

        AssertClaimedEntry(persistedFirstEntry, claimId, 1u, readStartedAt);
        AssertClaimedEntry(persistedSecondEntry, claimId, 1u, readStartedAt);
    }

    [Fact]
    public async Task Read_WithFailedEntriesBelowMaxDeliveryCount_ClaimsAndReadsTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.Failed, _settings.MaxDeliveryCount - 1, Guid.Empty, DateTime.MinValue);

        await SeedEntries(entry);

        DateTime readStartedAt = DateTime.UtcNow;

        // Act
        IReadOnlyCollection<OutboxEvent> result = await _sut.Read(1, CancellationToken.None);

        // Assert
        OutboxEvent outboxEvent = Assert.Single(result);
        Assert.Equal(entryId, outboxEvent.EventId);
        Assert.NotEqual(Guid.Empty, outboxEvent.ClaimId);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        AssertClaimedEntry(persistedEntry, outboxEvent.ClaimId, _settings.MaxDeliveryCount, readStartedAt);
    }

    [Fact]
    public async Task Read_WithInProcessingEntriesBelowMaxDeliveryCountAndExpiredClaim_ReclaimsAndReadsTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid previousClaimId = Guid.NewGuid();
        DateTime previousClaimExpiration = DateTime.UtcNow.AddMinutes(-2);

        OutboxEntry entry = CreateEntry(
            entryId,
            EntryState.InProcessing,
            _settings.MaxDeliveryCount - 1,
            previousClaimId,
            previousClaimExpiration);

        await SeedEntries(entry);

        DateTime readStartedAt = DateTime.UtcNow;

        // Act
        IReadOnlyCollection<OutboxEvent> result = await _sut.Read(1, CancellationToken.None);

        // Assert
        OutboxEvent outboxEvent = Assert.Single(result);
        Assert.Equal(entryId, outboxEvent.EventId);
        Assert.NotEqual(Guid.Empty, outboxEvent.ClaimId);
        Assert.NotEqual(previousClaimId, outboxEvent.ClaimId);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        AssertClaimedEntry(persistedEntry, outboxEvent.ClaimId, _settings.MaxDeliveryCount, readStartedAt);
        Assert.True(persistedEntry.ClaimExpiration > previousClaimExpiration);
    }

    [Fact]
    public async Task Read_WithProcessedEntries_DoesNotReadTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.Processed, 1, Guid.Empty, DateTime.MinValue);

        await SeedEntries(entry);

        // Act
        IReadOnlyCollection<OutboxEvent> result = await _sut.Read(1, CancellationToken.None);

        // Assert
        Assert.Empty(result);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task Read_WithFailedEntriesAboveMaxDeliveryCount_DoesNotReadTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.Failed, _settings.MaxDeliveryCount + 1u, Guid.Empty, DateTime.MinValue);

        await SeedEntries(entry);

        // Act
        IReadOnlyCollection<OutboxEvent> result = await _sut.Read(1, CancellationToken.None);

        // Assert
        Assert.Empty(result);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task Read_WithInProcessingEntriesAboveMaxDeliveryCountAndExpiredClaim_DoesNotReadTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(
            entryId,
            EntryState.InProcessing,
            _settings.MaxDeliveryCount + 1u,
            Guid.NewGuid(),
            DateTime.UtcNow.AddMinutes(-2));

        await SeedEntries(entry);

        // Act
        IReadOnlyCollection<OutboxEvent> result = await _sut.Read(1, CancellationToken.None);

        // Assert
        Assert.Empty(result);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task Read_WithInProcessingEntriesBelowMaxDeliveryCountAndActiveClaim_DoesNotReadTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(
            entryId,
            EntryState.InProcessing,
            _settings.MaxDeliveryCount - 1,
            Guid.NewGuid(),
            DateTime.UtcNow.AddMinutes(2));

        await SeedEntries(entry);

        // Act
        IReadOnlyCollection<OutboxEvent> result = await _sut.Read(1, CancellationToken.None);

        // Assert
        Assert.Empty(result);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task MarkAsDelivered_WithEntryInProcessingAndValidClaim_MarksTheEntryAsProcessed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid claimId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.InProcessing, 1, claimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsDelivered(entryId, claimId, CancellationToken.None);

        // Assert
        Assert.True(result);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        Assert.Equal(EntryState.Processed, persistedEntry.State);
        Assert.Equal(entry.DeliveryAttempts, persistedEntry.DeliveryAttempts);
        Assert.Equal(Guid.Empty, persistedEntry.ClaimId);
        Assert.Equal(DateTime.MinValue, persistedEntry.ClaimExpiration);
    }

    [Fact]
    public async Task MarkAsDelivered_WithEntryNotInProcessing_DoesNotMarkTheEntryAsProcessed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid claimId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.New, 1, claimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsDelivered(entryId, claimId, CancellationToken.None);

        // Assert
        Assert.False(result);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task MarkAsDelivered_WithEntryInProcessingAndInvalidClaim_DoesNotMarkTheEntryAsProcessed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid actualClaimId = Guid.NewGuid();
        Guid invalidClaimId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.InProcessing, 1, actualClaimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsDelivered(entryId, invalidClaimId, CancellationToken.None);

        // Assert
        Assert.False(result);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task MarkAsFailed_WithEntryInProcessingAndValidClaim_MarksTheEntryAsFailed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid claimId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.InProcessing, 1, claimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsFailed(entryId, claimId, CancellationToken.None);

        // Assert
        Assert.True(result);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        Assert.Equal(EntryState.Failed, persistedEntry.State);
        Assert.Equal(entry.DeliveryAttempts, persistedEntry.DeliveryAttempts);
        Assert.Equal(Guid.Empty, persistedEntry.ClaimId);
        Assert.Equal(DateTime.MinValue, persistedEntry.ClaimExpiration);
    }

    [Fact]
    public async Task MarkAsFailed_WithEntryNotInProcessing_DoesNotMarkTheEntryAsFailed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid claimId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.New, 1, claimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsFailed(entryId, claimId, CancellationToken.None);

        // Assert
        Assert.False(result);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task MarkAsFailed_WithEntryInProcessingAndInvalidClaim_DoesNotMarkTheEntryAsFailed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid actualClaimId = Guid.NewGuid();
        Guid invalidClaimId = Guid.NewGuid();
        OutboxEntry entry = CreateEntry(entryId, EntryState.InProcessing, 1, actualClaimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsFailed(entryId, invalidClaimId, CancellationToken.None);

        // Assert
        Assert.False(result);

        OutboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
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
            .SingleOrDefaultAsync(outboxEntry => outboxEntry.Id == id);

        Assert.NotNull(entry);
        return entry!;
    }

    private static void AssertClaimedEntry(OutboxEntry entry, Guid claimId, uint expectedDeliveryAttempts, DateTime readStartedAt)
    {
        Assert.Equal(EntryState.InProcessing, entry.State);
        Assert.Equal(expectedDeliveryAttempts, entry.DeliveryAttempts);
        Assert.Equal(claimId, entry.ClaimId);
        Assert.True(entry.ClaimExpiration > readStartedAt);
    }

    private static void AssertEntryUnchanged(OutboxEntry expected, OutboxEntry actual)
    {
        Assert.Equal(expected.State, actual.State);
        Assert.Equal(expected.DeliveryAttempts, actual.DeliveryAttempts);
        Assert.Equal(expected.ClaimId, actual.ClaimId);
    }

    private static OutboxEntry CreateEntry(
        Guid id,
        EntryState state,
        uint deliveryAttempts,
        Guid claimId,
        DateTime claimExpiration)
    {
        DateTime now = DateTime.UtcNow;
        return new OutboxEntry
        {
            Id = id,
            Event = "TestEvent",
            Handler = "TestHandler",
            Route = Route.Create().ToString(),
            Payload = "{}",
            State = state,
            Created = now.AddMinutes(-1),
            Updated = now,
            DeliveryAttempts = deliveryAttempts,
            ClaimId = claimId,
            ClaimExpiration = claimExpiration
        };
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
