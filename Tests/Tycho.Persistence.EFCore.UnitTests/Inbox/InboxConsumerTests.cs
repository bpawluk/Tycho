using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tycho.Events;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Common;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.UnitTests._Data.Events;

namespace Tycho.Persistence.EFCore.UnitTests.Inbox;

public sealed class InboxConsumerTests : IAsyncLifetime
{
    private readonly InboxConsumerSettings _settings = new()
    {
        MaxProcessingCount = 3,
        ProcessingExpiration = TimeSpan.FromMinutes(5)
    };

    private TestDbContext _dbContext = default!;
    private SqliteConnection _connection = default!;

    private Mock<IEventSerializer> _eventSerializerMock = default!;
    private InboxConsumer _sut = default!;

    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        DbContextOptionsBuilder<TestDbContext> optionsBuilder = new();
        DbContextOptions<TestDbContext> options = optionsBuilder.UseSqlite(_connection).Options;

        _dbContext = new TestDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        _eventSerializerMock = new Mock<IEventSerializer>();
        _eventSerializerMock
            .Setup(serializer => serializer.Deserialize(It.IsAny<SerializedRoutedEvent>()))
            .Returns<SerializedRoutedEvent>(CreateRoutedEvent);

        _sut = new InboxConsumer(_eventSerializerMock.Object, _dbContext, _settings);
    }

    [Fact]
    public async Task TryReadAsync_WithNewEntries_ClaimsAndReadsTheOldestEntry()
    {
        // Arrange
        Guid firstEntryId = Guid.NewGuid();
        Guid secondEntryId = Guid.NewGuid();
        InboxEntry firstEntry = CreateEntry(firstEntryId, EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        InboxEntry secondEntry = CreateEntry(secondEntryId, EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        secondEntry.Created = firstEntry.Created.AddSeconds(1);

        await SeedEntries(firstEntry, secondEntry);

        DateTime readStartedAt = DateTime.UtcNow;

        // Act
        InboxEvent? result = await _sut.TryReadAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(firstEntryId, result.EventId);
        Guid claimId = result.ClaimId;
        Assert.NotEqual(Guid.Empty, claimId);

        InboxEntry persistedFirstEntry = await LoadEntry(firstEntryId);
        InboxEntry persistedSecondEntry = await LoadEntry(secondEntryId);

        AssertClaimedEntry(persistedFirstEntry, claimId, 1u, readStartedAt);
        AssertEntryUnchanged(secondEntry, persistedSecondEntry);

        InboxEvent? nextResult = await _sut.TryReadAsync(CancellationToken.None);
        Assert.NotNull(nextResult);
        Assert.Equal(secondEntryId, nextResult.EventId);
        Assert.NotEqual(claimId, nextResult.ClaimId);

        persistedSecondEntry = await LoadEntry(secondEntryId);
        AssertClaimedEntry(persistedSecondEntry, nextResult.ClaimId, 1u, readStartedAt);
    }

    [Fact]
    public async Task Read_WithFailedEntriesBelowMaxProcessingCount_ClaimsAndReadsTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(entryId, EntryState.Failed, _settings.MaxProcessingCount - 1, Guid.Empty, DateTime.MinValue);

        await SeedEntries(entry);

        DateTime readStartedAt = DateTime.UtcNow;

        // Act
        InboxEvent? result = await _sut.TryReadAsync(CancellationToken.None);

        // Assert
        InboxEvent inboxEvent = Assert.IsType<InboxEvent>(result);
        Assert.Equal(entryId, inboxEvent.EventId);
        Assert.NotEqual(Guid.Empty, inboxEvent.ClaimId);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        AssertClaimedEntry(persistedEntry, inboxEvent.ClaimId, _settings.MaxProcessingCount, readStartedAt);
    }

    [Fact]
    public async Task Read_WithInProcessingEntriesBelowMaxProcessingCountAndExpiredClaim_ReclaimsAndReadsTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid previousClaimId = Guid.NewGuid();
        DateTime previousClaimExpiration = DateTime.UtcNow.AddMinutes(-2);

        InboxEntry entry = CreateEntry(
            entryId,
            EntryState.InProcessing,
            _settings.MaxProcessingCount - 1,
            previousClaimId,
            previousClaimExpiration);

        await SeedEntries(entry);

        DateTime readStartedAt = DateTime.UtcNow;

        // Act
        InboxEvent? result = await _sut.TryReadAsync(CancellationToken.None);

        // Assert
        InboxEvent inboxEvent = Assert.IsType<InboxEvent>(result);
        Assert.Equal(entryId, inboxEvent.EventId);
        Assert.NotEqual(Guid.Empty, inboxEvent.ClaimId);
        Assert.NotEqual(previousClaimId, inboxEvent.ClaimId);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        AssertClaimedEntry(persistedEntry, inboxEvent.ClaimId, _settings.MaxProcessingCount, readStartedAt);
        Assert.True(persistedEntry.ClaimExpiration > previousClaimExpiration);
    }

    [Fact]
    public async Task Read_WithProcessedEntries_DoesNotReadTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(entryId, EntryState.Processed, 1, Guid.Empty, DateTime.MinValue);

        await SeedEntries(entry);

        // Act
        InboxEvent? result = await _sut.TryReadAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task Read_WithFailedEntriesAboveMaxProcessingCount_DoesNotReadTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(entryId, EntryState.Failed, _settings.MaxProcessingCount + 1u, Guid.Empty, DateTime.MinValue);

        await SeedEntries(entry);

        // Act
        InboxEvent? result = await _sut.TryReadAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task Read_WithInProcessingEntriesAboveMaxProcessingCountAndExpiredClaim_DoesNotReadTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(
            entryId,
            EntryState.InProcessing,
            _settings.MaxProcessingCount + 1u,
            Guid.NewGuid(),
            DateTime.UtcNow.AddMinutes(-2));

        await SeedEntries(entry);

        // Act
        InboxEvent? result = await _sut.TryReadAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task Read_WithInProcessingEntriesBelowMaxProcessingCountAndActiveClaim_DoesNotReadTheEntries()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(
            entryId,
            EntryState.InProcessing,
            _settings.MaxProcessingCount - 1,
            Guid.NewGuid(),
            DateTime.UtcNow.AddMinutes(2));

        await SeedEntries(entry);

        // Act
        InboxEvent? result = await _sut.TryReadAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task TryReadAsync_WithEntryDeserializationFailure_MarksFailedAndReturnsNull()
    {
        // Arrange
        Guid failingEntryId = Guid.NewGuid();
        Guid successfulEntryId = Guid.NewGuid();
        InboxEntry failingEntry = CreateEntry(failingEntryId, EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        InboxEntry successfulEntry = CreateEntry(successfulEntryId, EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        successfulEntry.Created = failingEntry.Created.AddSeconds(1);

        await SeedEntries(failingEntry, successfulEntry);

        _eventSerializerMock
            .Setup(serializer => serializer.Deserialize(It.Is<SerializedRoutedEvent>(evt => evt.Id == failingEntryId)))
            .Throws(new InvalidOperationException("deserialize failure"));

        // Act
        InboxEvent? result = await _sut.TryReadAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);

        InboxEntry persistedFailingEntry = await LoadEntry(failingEntryId);
        Assert.Equal(EntryState.Failed, persistedFailingEntry.State);
        Assert.Equal(1u, persistedFailingEntry.ProcessingAttempts);
        Assert.Equal(Guid.Empty, persistedFailingEntry.ClaimId);
        Assert.Equal(DateTime.MinValue, persistedFailingEntry.ClaimExpiration);

        InboxEntry persistedSuccessfulEntry = await LoadEntry(successfulEntryId);
        AssertEntryUnchanged(successfulEntry, persistedSuccessfulEntry);
    }

    [Fact]
    public async Task MarkAsHandledAsync_WithEntryInProcessingAndValidClaim_MarksTheEntryAsProcessed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid claimId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(entryId, EntryState.InProcessing, 1, claimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsHandledAsync(claimId, CancellationToken.None);

        // Assert
        Assert.True(result);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        Assert.Equal(EntryState.Processed, persistedEntry.State);
        Assert.Equal(entry.ProcessingAttempts, persistedEntry.ProcessingAttempts);
        Assert.Equal(Guid.Empty, persistedEntry.ClaimId);
        Assert.Equal(DateTime.MinValue, persistedEntry.ClaimExpiration);
    }

    [Fact]
    public async Task MarkAsHandledAsync_WithEntryNotInProcessing_DoesNotMarkTheEntryAsProcessed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid claimId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(entryId, EntryState.New, 1, claimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsHandledAsync(claimId, CancellationToken.None);

        // Assert
        Assert.False(result);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task MarkAsHandledAsync_WithEntryInProcessingAndInvalidClaim_DoesNotMarkTheEntryAsProcessed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid actualClaimId = Guid.NewGuid();
        Guid invalidClaimId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(entryId, EntryState.InProcessing, 1, actualClaimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsHandledAsync(invalidClaimId, CancellationToken.None);

        // Assert
        Assert.False(result);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task MarkAsFailedAsync_WithEntryInProcessingAndValidClaim_MarksTheEntryAsFailed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid claimId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(entryId, EntryState.InProcessing, 1, claimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsFailedAsync(claimId, CancellationToken.None);

        // Assert
        Assert.True(result);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        Assert.Equal(EntryState.Failed, persistedEntry.State);
        Assert.Equal(entry.ProcessingAttempts, persistedEntry.ProcessingAttempts);
        Assert.Equal(Guid.Empty, persistedEntry.ClaimId);
        Assert.Equal(DateTime.MinValue, persistedEntry.ClaimExpiration);
    }

    [Fact]
    public async Task MarkAsFailedAsync_WithEntryNotInProcessing_DoesNotMarkTheEntryAsFailed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid claimId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(entryId, EntryState.New, 1, claimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsFailedAsync(claimId, CancellationToken.None);

        // Assert
        Assert.False(result);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    [Fact]
    public async Task MarkAsFailedAsync_WithEntryInProcessingAndInvalidClaim_DoesNotMarkTheEntryAsFailed()
    {
        // Arrange
        Guid entryId = Guid.NewGuid();
        Guid actualClaimId = Guid.NewGuid();
        Guid invalidClaimId = Guid.NewGuid();
        InboxEntry entry = CreateEntry(entryId, EntryState.InProcessing, 1, actualClaimId, DateTime.UtcNow.AddMinutes(1));

        await SeedEntries(entry);

        // Act
        bool result = await _sut.MarkAsFailedAsync(invalidClaimId, CancellationToken.None);

        // Assert
        Assert.False(result);

        InboxEntry persistedEntry = await LoadEntry(entryId);
        AssertEntryUnchanged(entry, persistedEntry);
    }

    private async Task SeedEntries(params InboxEntry[] entries)
    {
        _dbContext.Set<InboxEntry>().AddRange(entries);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<InboxEntry> LoadEntry(Guid id)
    {
        _dbContext.ChangeTracker.Clear();

        InboxEntry? entry = await _dbContext
            .Set<InboxEntry>()
            .AsNoTracking()
            .SingleOrDefaultAsync(inboxEntry => inboxEntry.Id == id);

        Assert.NotNull(entry);
        return entry!;
    }

    private static void AssertClaimedEntry(InboxEntry entry, Guid claimId, uint expectedProcessingAttempts, DateTime readStartedAt)
    {
        Assert.Equal(EntryState.InProcessing, entry.State);
        Assert.Equal(expectedProcessingAttempts, entry.ProcessingAttempts);
        Assert.Equal(claimId, entry.ClaimId);
        Assert.True(entry.ClaimExpiration > readStartedAt);
    }

    private static void AssertEntryUnchanged(InboxEntry expected, InboxEntry actual)
    {
        Assert.Equal(expected.State, actual.State);
        Assert.Equal(expected.ProcessingAttempts, actual.ProcessingAttempts);
        Assert.Equal(expected.ClaimId, actual.ClaimId);
        Assert.Equal(expected.ClaimExpiration, actual.ClaimExpiration);
    }

    private static InboxEntry CreateEntry(
        Guid id,
        EntryState state,
        uint processingAttempts,
        Guid claimId,
        DateTime claimExpiration)
    {
        DateTime now = DateTime.UtcNow;
        return new InboxEntry
        {
            Id = id,
            PublishId = Guid.NewGuid(),
            Event = EventIdentity.Create<TestEvent>().ToString(),
            Handler = EventHandlerIdentity.Create<TestEventHandler>().ToString(),
            Payload = "{}",
            State = state,
            Created = now.AddMinutes(-1),
            Updated = now,
            ProcessingAttempts = processingAttempts,
            ClaimId = claimId,
            ClaimExpiration = claimExpiration
        };
    }

    private static RoutedEvent CreateRoutedEvent(SerializedRoutedEvent serializedEvent)
    {
        return new RoutedEvent<TestEvent>(
            serializedEvent.Id,
            serializedEvent.PublishId,
            serializedEvent.EventId,
            serializedEvent.HandlerId,
            serializedEvent.Route,
            new TestEvent());
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);

    private sealed class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(EventContext<TestEvent> context, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
