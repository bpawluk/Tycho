using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.UnitTests._Data.Events;

namespace Tycho.Persistence.EFCore.UnitTests.Inbox;

public sealed class InboxWriterTests : IAsyncLifetime
{
    private SqliteConnection _connection = default!;
    private DbContextOptions<TestDbContext> _dbContextOptions = default!;
    private TestDbContext _dbContext = default!;
    private InboxActivity _inboxActivity = default!;
    private InboxWriter _sut = default!;

    private int _inboxActivityNotificationCount;

    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        _dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new TestDbContext(_dbContextOptions);
        await _dbContext.Database.EnsureCreatedAsync();

        _inboxActivity = new InboxActivity();
        _inboxActivity.NewEntriesAdded += (_, _) => _inboxActivityNotificationCount++;
        _sut = new InboxWriter(_inboxActivity, _dbContext);
    }

    [Fact]
    public async Task Write_WithNewEntry_PersistsEntryAndNotifiesActivity()
    {
        // Arrange
        SerializedRoutedEvent serializedEvent = CreateSerializedEvent();

        // Act
        await _sut.Write(serializedEvent, TestContext.Current.CancellationToken);

        // Assert
        InboxEntry persistedEntry = await LoadEntry(serializedEvent.Id);
        Assert.Equal(serializedEvent.Id, persistedEntry.Id);
        Assert.Equal(serializedEvent.PublishId, persistedEntry.PublishId);
        Assert.Equal(serializedEvent.EventId.ToString(), persistedEntry.Event);
        Assert.Equal(serializedEvent.HandlerId.ToString(), persistedEntry.Handler);
        Assert.Equal(serializedEvent.Payload, persistedEntry.Payload);
        Assert.Equal(1, _inboxActivityNotificationCount);
    }

    [Fact]
    public async Task Write_WithMatchingExistingEntry_TreatsReceiptAsSuccessful()
    {
        // Arrange
        SerializedRoutedEvent serializedEvent = CreateSerializedEvent();
        await _sut.Write(serializedEvent, TestContext.Current.CancellationToken);
        await using var retryDbContext = new TestDbContext(_dbContextOptions);
        var retryWriter = new InboxWriter(_inboxActivity, retryDbContext);

        // Act
        await retryWriter.Write(serializedEvent, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, await CountPersistedEntries());
        Assert.Empty(retryDbContext.ChangeTracker.Entries());
        Assert.Equal(2, _inboxActivityNotificationCount);
    }

    [Fact]
    public async Task Write_WithExistingEntryForDifferentPublishId_RethrowsPersistenceFailure()
    {
        // Arrange
        SerializedRoutedEvent persistedEvent = CreateSerializedEvent();
        await _sut.Write(persistedEvent, TestContext.Current.CancellationToken);
        SerializedRoutedEvent conflictingEvent = CreateSerializedEvent(id: persistedEvent.Id);
        await using var retryDbContext = new TestDbContext(_dbContextOptions);
        var retryWriter = new InboxWriter(_inboxActivity, retryDbContext);

        // Act
        Task act() => retryWriter.Write(conflictingEvent, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(act);
        Assert.Equal(1, await CountPersistedEntries());
        Assert.Empty(retryDbContext.ChangeTracker.Entries());
        Assert.Equal(1, _inboxActivityNotificationCount);
    }

    [Fact]
    public async Task Write_WithExistingEntryContainingDifferentPayload_RethrowsPersistenceFailure()
    {
        // Arrange
        SerializedRoutedEvent persistedEvent = CreateSerializedEvent();
        await _sut.Write(persistedEvent, TestContext.Current.CancellationToken);
        SerializedRoutedEvent conflictingEvent = CreateSerializedEvent(
            persistedEvent.Id,
            persistedEvent.PublishId,
            "different payload");
        await using var retryDbContext = new TestDbContext(_dbContextOptions);
        var retryWriter = new InboxWriter(_inboxActivity, retryDbContext);

        // Act
        Task act() => retryWriter.Write(conflictingEvent, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(act);
        Assert.Equal(1, await CountPersistedEntries());
        Assert.Empty(retryDbContext.ChangeTracker.Entries());
        Assert.Equal(1, _inboxActivityNotificationCount);
    }

    [Fact]
    public async Task Write_WhenUpdateFailsWithoutExistingEntry_RethrowsPersistenceFailure()
    {
        // Arrange
        var persistenceFailure = new DbUpdateException("persistence failure");
        DbContextOptions<FailingDbContext> options = new DbContextOptionsBuilder<FailingDbContext>()
            .UseSqlite(_connection)
            .Options;
        await using var dbContext = new FailingDbContext(options);
        await dbContext.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        dbContext.SaveFailure = persistenceFailure;
        var sut = new InboxWriter(_inboxActivity, dbContext);

        // Act
        Task act() => sut.Write(CreateSerializedEvent(), TestContext.Current.CancellationToken);

        // Assert
        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(act);
        Assert.Same(persistenceFailure, exception);
        Assert.Empty(dbContext.ChangeTracker.Entries());
        Assert.Equal(0, _inboxActivityNotificationCount);
    }

    [Fact]
    public async Task Write_WhenPersistenceFailsWithUnexpectedException_DoesNotNotifyActivity()
    {
        // Arrange
        var persistenceFailure = new InvalidOperationException("persistence failure");
        DbContextOptions<FailingDbContext> options = new DbContextOptionsBuilder<FailingDbContext>()
            .UseSqlite(_connection)
            .Options;
        await using var dbContext = new FailingDbContext(options);
        await dbContext.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        dbContext.SaveFailure = persistenceFailure;
        var sut = new InboxWriter(_inboxActivity, dbContext);

        // Act
        Task act() => sut.Write(CreateSerializedEvent(), TestContext.Current.CancellationToken);

        // Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(persistenceFailure, exception);
        Assert.Equal(0, _inboxActivityNotificationCount);
    }

    private async Task<InboxEntry> LoadEntry(Guid entryId)
    {
        return await _dbContext.Set<InboxEntry>()
            .AsNoTracking()
            .SingleAsync(entry => entry.Id == entryId);
    }

    private async Task<int> CountPersistedEntries()
    {
        await using var verificationDbContext = new TestDbContext(_dbContextOptions);
        return await verificationDbContext.Set<InboxEntry>().AsNoTracking().CountAsync();
    }

    private static SerializedRoutedEvent CreateSerializedEvent(
        Guid? id = null,
        Guid? publishId = null,
        string payload = "{}")
    {
        return new SerializedRoutedEvent(
            id ?? Guid.NewGuid(),
            publishId ?? Guid.NewGuid(),
            EventIdentity.Create<TestEvent>(),
            EventHandlerIdentity.Parse("test-handler"),
            Route.Empty(),
            payload);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);

    private sealed class FailingDbContext(DbContextOptions<FailingDbContext> options) : TychoDbContext(options)
    {
        public override string InboxTableName => "TestInbox";
        public override string OutboxTableName => "TestOutbox";

        public Exception? SaveFailure { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveFailure is null
                ? base.SaveChangesAsync(cancellationToken)
                : Task.FromException<int>(SaveFailure);
        }
    }
}
