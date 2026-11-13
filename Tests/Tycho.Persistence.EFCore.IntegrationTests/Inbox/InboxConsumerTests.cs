using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Serialization;
using Tycho.Events;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Events;
using Tycho.Persistence.EFCore.TestUtils.Logging;
using Tycho.Persistence.EFCore.TestUtils.Other;
using Tycho.Structure;

namespace Tycho.Persistence.EFCore.IntegrationTests.Inbox;

public sealed class InboxConsumerTests(DatabaseServers servers) : DatabaseTest(servers)
{
    private readonly InboxConsumerSettings _settings = new()
    {
        MaxProcessingCount = 3,
        ProcessingExpiration = TimeSpan.FromMinutes(5)
    };

    private TestDbContext _dbContext = default!;

    private Mock<IEventSerializer> _eventSerializerMock = default!;
    private InboxConsumer _sut = default!;

    private async Task SetupAsync(DatabaseProvider provider)
    {
        await InitializeDatabaseAsync(provider);

        DbContextOptions<TestDbContext> options = Options<TestDbContext>().Options;

        _dbContext = new TestDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        _eventSerializerMock = new Mock<IEventSerializer>();
        _eventSerializerMock
            .Setup(serializer => serializer.Deserialize(It.IsAny<SerializedEvent>()))
            .Returns<SerializedEvent>(CreateEvent);

        var persistenceOwner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
        _sut = new InboxConsumer(_eventSerializerMock.Object, _dbContext, persistenceOwner, _settings);
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
        InboxEntry entry = CreateEntry(Guid.NewGuid(), EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        entry.Handler = "test-handler";
        entry.Payload = "{\"value\":1}";
        await SeedEntries(entry);

        // Act
        InboxEvent? result = await _sut.TryReadAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.ClaimId);
        AssertDeserializedEvent(entry, result.Event);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task MarkAsHandledAsync_WithReadEvent_UpdatesEntryStatus(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry(Guid.NewGuid(), EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        await SeedEntries(entry);
        InboxEvent? result = await _sut.TryReadAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(result);

        // Act
        bool updated = await _sut.MarkAsHandledAsync(result.EventId, result.ClaimId, TestContext.Current.CancellationToken);

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
        InboxEntry entry = CreateEntry(Guid.NewGuid(), EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        await SeedEntries(entry);
        InboxEvent? result = await _sut.TryReadAsync(TestContext.Current.CancellationToken);
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
    public async Task TryReadAsync_WithEntryDeserializationFailure_MarksFailedAndThrows(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        Guid failingEntryId = Guid.NewGuid();
        Guid successfulEntryId = Guid.NewGuid();
        InboxEntry failingEntry = CreateEntry(failingEntryId, EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        InboxEntry successfulEntry = CreateEntry(successfulEntryId, EntryState.New, 0, Guid.Empty, DateTime.MinValue);
        successfulEntry.Updated = failingEntry.Updated.AddSeconds(1);

        await SeedEntries(failingEntry, successfulEntry);

        _eventSerializerMock
            .Setup(serializer => serializer.Deserialize(It.Is<SerializedEvent>(evt => evt.Id == failingEntryId)))
            .Throws(new InvalidOperationException("deserialize failure"));

        // Act
        Task Act() => _sut.TryReadAsync(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);

        InboxEntry persistedFailingEntry = await LoadEntry(failingEntryId);
        Assert.Equal(EntryState.Failed, persistedFailingEntry.State);
        Assert.Equal(1u, persistedFailingEntry.Attempts);
        Assert.Equal(Guid.Empty, persistedFailingEntry.ClaimId);
        Assert.Equal(DateTime.MinValue, persistedFailingEntry.ClaimExpiration);

        InboxEntry persistedSuccessfulEntry = await LoadEntry(successfulEntryId);
        AssertEntryUnchanged(successfulEntry, persistedSuccessfulEntry);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task TryReadAsync_WhenEntryDisappearsDuringDeserialization_LogsFailedStatusUpdateAndRethrowsOriginalError(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        Guid entryId = Guid.NewGuid();
        await SeedEntries(CreateEntry(entryId, EntryState.New, 0, Guid.Empty, DateTime.MinValue));

        var logger = new Mock<ILogger<InboxConsumer>>();
        logger.Setup(item => item.IsEnabled(LogLevel.Warning)).Returns(true);

        var owner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
        _sut = new InboxConsumer(_eventSerializerMock.Object, _dbContext, owner, _settings, logger.Object);

        var deserializeError = new InvalidOperationException("deserialize failure");
        Guid claimedId = Guid.Empty;
        _eventSerializerMock
            .Setup(serializer => serializer.Deserialize(It.IsAny<SerializedEvent>()))
            .Returns<SerializedEvent>(serializedEvent =>
            {
                claimedId = _dbContext.Set<InboxEntry>().AsNoTracking().Single(entry => entry.EntryId == serializedEvent.Id).ClaimId;
                _dbContext.Set<InboxEntry>().Where(entry => entry.EntryId == serializedEvent.Id).ExecuteDelete();
                throw deserializeError;
            });

        // Act
        InvalidOperationException actualError = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.TryReadAsync(TestContext.Current.CancellationToken));

        // Assert
        Assert.Same(deserializeError, actualError);
        Assert.NotEqual(Guid.Empty, claimedId);
        LogAssert.Logged(logger, LogLevel.Warning, 2302, "InboxMessageStatusUpdateFailed", null, ("EntryId", entryId), ("ClaimId", claimedId));
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
        InboxEvent? result = await _sut.TryReadAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
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
            .SingleOrDefaultAsync(inboxEntry => inboxEntry.EntryId == id);

        Assert.NotNull(entry);
        return entry!;
    }

    private static void AssertEntryUnchanged(InboxEntry expected, InboxEntry actual)
    {
        Assert.Equal(expected.State, actual.State);
        Assert.Equal(expected.Attempts, actual.Attempts);
        Assert.Equal(expected.ClaimId, actual.ClaimId);
        Assert.Equal(expected.ClaimExpiration, actual.ClaimExpiration);
    }

    private void AssertDeserializedEvent(InboxEntry expected, Event actual)
    {
        Assert.Equal(expected.EntryId, actual.Id);
        Assert.Equal(expected.PublishId, actual.PublishId);
        Assert.Equal(expected.Event, actual.EventId.Value);
        Assert.Equal(expected.Handler, actual.HandlerId.Value);
        _eventSerializerMock.Verify(serializer => serializer.Deserialize(It.Is<SerializedEvent>(serializedEvent =>
            serializedEvent.Id == expected.EntryId &&
            serializedEvent.PublishId == expected.PublishId &&
            serializedEvent.EventId.Value == expected.Event &&
            serializedEvent.HandlerId.Value == expected.Handler &&
            serializedEvent.Payload == expected.Payload)), Times.Once);
    }

    private static InboxEntry CreateEntry(
        Guid id,
        EntryState state,
        uint processingAttempts,
        Guid claimId,
        DateTime claimExpiration)
    {
        DateTime now = DateTime.UtcNow;
        var persistenceOwner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
        return new InboxEntry
        {
            OwnerId = persistenceOwner.Identifier,
            EntryId = id,
            PublishId = Guid.NewGuid(),
            Handler = EventHandlerIdentity.Create<TestEventHandler>().ToString(),
            Event = EventIdentity.Create<TestEvent>().ToString(),
            Payload = "{}",
            State = state,
            Created = now.AddMinutes(-1),
            Updated = now,
            Attempts = processingAttempts,
            ClaimId = claimId,
            ClaimExpiration = claimExpiration
        };
    }

    private static Event CreateEvent(SerializedEvent serializedEvent)
    {
        return new Event<TestEvent>(
            serializedEvent.Id,
            serializedEvent.PublishId,
            serializedEvent.EventId,
            serializedEvent.HandlerId,
            new TestEvent());
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

    private sealed class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(EventContext<TestEvent> context, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
