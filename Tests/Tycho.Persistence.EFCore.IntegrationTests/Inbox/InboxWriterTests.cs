using Microsoft.EntityFrameworkCore;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Events;
using Tycho.Persistence.EFCore.TestUtils.Other;

namespace Tycho.Persistence.EFCore.IntegrationTests.Inbox;

public sealed class InboxWriterTests(DatabaseServers servers) : DatabaseTest(servers)
{
    private DbContextOptions<TestDbContext> _dbContextOptions = default!;
    private TestDbContext _dbContext = default!;
    private InboxActivity _inboxActivity = default!;
    private PersistenceOwner _persistenceOwner = default!;
    private InboxWriter _sut = default!;

    private int _inboxActivityNotificationCount;

    private async Task SetupAsync(DatabaseProvider provider)
    {
        await InitializeDatabaseAsync(provider);

        _dbContextOptions = Options<TestDbContext>()
            .Options;

        _dbContext = new TestDbContext(_dbContextOptions);
        await _dbContext.Database.EnsureCreatedAsync();

        _inboxActivity = new InboxActivity();
        _inboxActivity.NewEntriesAdded += (_, _) => _inboxActivityNotificationCount++;

        _persistenceOwner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
        _sut = new InboxWriter(_inboxActivity, _dbContext, _persistenceOwner);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WithNewEntry_PersistsEntryAndNotifiesActivity(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        SerializedEvent serializedEvent = CreateSerializedEvent();

        // Act
        await _sut.Write(serializedEvent, TestContext.Current.CancellationToken);

        // Assert
        InboxEntry persistedEntry = await LoadEntry(serializedEvent.Id);
        Assert.Equal(serializedEvent.Id, persistedEntry.EntryId);
        Assert.Equal(serializedEvent.PublishId, persistedEntry.PublishId);
        Assert.Equal(serializedEvent.HandlerId.ToString(), persistedEntry.Handler);
        Assert.Equal(serializedEvent.EventId.ToString(), persistedEntry.Event);
        Assert.Equal(serializedEvent.Payload, persistedEntry.Payload);
        Assert.Equal(1, _inboxActivityNotificationCount);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WithMatchingExistingEntry_TreatsReceiptAsSuccessful(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        SerializedEvent serializedEvent = CreateSerializedEvent();
        await _sut.Write(serializedEvent, TestContext.Current.CancellationToken);
        await using var retryDbContext = new TestDbContext(_dbContextOptions);
        InboxWriter retryWriter = new InboxWriter(_inboxActivity, retryDbContext, _persistenceOwner);

        // Act
        await retryWriter.Write(serializedEvent, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, await CountPersistedEntries());
        Assert.Empty(retryDbContext.ChangeTracker.Entries());
        Assert.Equal(2, _inboxActivityNotificationCount);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WithExistingEntryForDifferentPublishId_RethrowsPersistenceFailure(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        SerializedEvent persistedEvent = CreateSerializedEvent();
        await _sut.Write(persistedEvent, TestContext.Current.CancellationToken);
        SerializedEvent conflictingEvent = CreateSerializedEvent(id: persistedEvent.Id);
        await using var retryDbContext = new TestDbContext(_dbContextOptions);
        InboxWriter retryWriter = new InboxWriter(_inboxActivity, retryDbContext, _persistenceOwner);

        // Act
        Task act() => retryWriter.Write(conflictingEvent, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(act);
        Assert.Equal(1, await CountPersistedEntries());
        Assert.Empty(retryDbContext.ChangeTracker.Entries());
        Assert.Equal(1, _inboxActivityNotificationCount);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WithExistingEntryContainingDifferentPayload_KeepsFirstPayload(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        SerializedEvent persistedEvent = CreateSerializedEvent();
        await _sut.Write(persistedEvent, TestContext.Current.CancellationToken);
        SerializedEvent conflictingEvent = CreateSerializedEvent(
            persistedEvent.Id,
            persistedEvent.PublishId,
            "different payload");
        await using var retryDbContext = new TestDbContext(_dbContextOptions);
        InboxWriter retryWriter = new InboxWriter(_inboxActivity, retryDbContext, _persistenceOwner);

        // Act
        await retryWriter.Write(conflictingEvent, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, await CountPersistedEntries());
        Assert.Empty(retryDbContext.ChangeTracker.Entries());
        Assert.Equal(persistedEvent.Payload, (await LoadEntry(persistedEvent.Id)).Payload);
        Assert.Equal(2, _inboxActivityNotificationCount);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WithClearedExistingPayload_PreservesDeduplication(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        SerializedEvent serializedEvent = CreateSerializedEvent();
        await _sut.Write(serializedEvent, TestContext.Current.CancellationToken);
        await _dbContext.Set<InboxEntry>().ExecuteUpdateAsync(setters => setters
            .SetProperty(entry => entry.State, EntryState.Processed)
            .SetProperty(entry => entry.Payload, "{}"), TestContext.Current.CancellationToken);
        await using var retryDbContext = new TestDbContext(_dbContextOptions);
        InboxWriter retryWriter = new InboxWriter(_inboxActivity, retryDbContext, _persistenceOwner);

        // Act
        await retryWriter.Write(serializedEvent, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, await CountPersistedEntries());
        InboxEntry entry = await LoadEntry(serializedEvent.Id);
        Assert.Equal("{}", entry.Payload);
        Assert.Equal(EntryState.Processed, entry.State);
        Assert.Empty(retryDbContext.ChangeTracker.Entries());
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WhenUpdateFailsWithoutExistingEntry_RethrowsPersistenceFailure(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        var persistenceFailure = new DbUpdateException("persistence failure");
        await using FailingDbContext dbContext = CreateFailingDbContext();
        dbContext.SaveFailure = persistenceFailure;
        InboxWriter sut = new InboxWriter(_inboxActivity, dbContext, _persistenceOwner);

        // Act
        Task act() => sut.Write(CreateSerializedEvent(), TestContext.Current.CancellationToken);

        // Assert
        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(act);
        Assert.Same(persistenceFailure, exception);
        Assert.Empty(dbContext.ChangeTracker.Entries());
        Assert.Equal(0, _inboxActivityNotificationCount);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WhenPersistenceFailsWithUnexpectedException_DoesNotNotifyActivity(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        var persistenceFailure = new InvalidOperationException("persistence failure");
        await using FailingDbContext dbContext = CreateFailingDbContext();
        dbContext.SaveFailure = persistenceFailure;
        InboxWriter sut = new InboxWriter(_inboxActivity, dbContext, _persistenceOwner);

        // Act
        Task act() => sut.Write(CreateSerializedEvent(), TestContext.Current.CancellationToken);

        // Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(persistenceFailure, exception);
        Assert.Equal(0, _inboxActivityNotificationCount);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WhenUpdateFailsWithSameOwnerOnlyEntry_RethrowsPersistenceFailure(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        SerializedEvent serializedEvent = CreateSerializedEvent();
        await using FailingDbContext dbContext = CreateFailingDbContext();
        InboxEntry decoy = CreateDecoyEntry(serializedEvent, sameOwner: true);
        dbContext.Set<InboxEntry>().Add(decoy);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        dbContext.ChangeTracker.Clear();

        var persistenceFailure = new DbUpdateException("persistence failure");
        dbContext.SaveFailure = persistenceFailure;
        InboxWriter sut = new InboxWriter(_inboxActivity, dbContext, _persistenceOwner);

        // Act
        Task Act() => sut.Write(serializedEvent, TestContext.Current.CancellationToken);

        // Assert
        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(Act);
        Assert.Same(persistenceFailure, exception);
        Assert.Empty(dbContext.ChangeTracker.Entries());
        InboxEntry persistedEntry = await _dbContext.Set<InboxEntry>().AsNoTracking()
            .SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(decoy.OwnerId, persistedEntry.OwnerId);
        Assert.Equal(decoy.EntryId, persistedEntry.EntryId);
        Assert.Equal(decoy.Payload, persistedEntry.Payload);
        Assert.Equal(0, _inboxActivityNotificationCount);
        }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WhenUpdateFailsWithSameEntryIdOnlyEntry_RethrowsPersistenceFailure(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        SerializedEvent serializedEvent = CreateSerializedEvent();
        await using FailingDbContext dbContext = CreateFailingDbContext();
        InboxEntry decoy = CreateDecoyEntry(serializedEvent, sameOwner: false);
        dbContext.Set<InboxEntry>().Add(decoy);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        dbContext.ChangeTracker.Clear();

        var persistenceFailure = new DbUpdateException("persistence failure");
        dbContext.SaveFailure = persistenceFailure;
        InboxWriter sut = new InboxWriter(_inboxActivity, dbContext, _persistenceOwner);

        // Act
        Task Act() => sut.Write(serializedEvent, TestContext.Current.CancellationToken);

        // Assert
        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(Act);
        Assert.Same(persistenceFailure, exception);
        Assert.Empty(dbContext.ChangeTracker.Entries());
        InboxEntry persistedEntry = await _dbContext.Set<InboxEntry>().AsNoTracking()
            .SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(decoy.OwnerId, persistedEntry.OwnerId);
        Assert.Equal(decoy.EntryId, persistedEntry.EntryId);
        Assert.Equal(decoy.Payload, persistedEntry.Payload);
        Assert.Equal(0, _inboxActivityNotificationCount);
        }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WithMatchingEntryAndSameOwnerOnlyDecoy_TreatsReceiptAsSuccessful(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        SerializedEvent serializedEvent = CreateSerializedEvent();
        await _sut.Write(serializedEvent, TestContext.Current.CancellationToken);
        await using var retryDbContext = new TestDbContext(_dbContextOptions);
        InboxEntry decoy = CreateDecoyEntry(serializedEvent, sameOwner: true);
        retryDbContext.Set<InboxEntry>().Add(decoy);
        await retryDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        retryDbContext.ChangeTracker.Clear();
        InboxWriter retryWriter = new InboxWriter(_inboxActivity, retryDbContext, _persistenceOwner);

        // Act
        await retryWriter.Write(serializedEvent, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, await CountPersistedEntries());
        Assert.Empty(retryDbContext.ChangeTracker.Entries());
        InboxEntry persistedEntry = await _dbContext.Set<InboxEntry>().AsNoTracking()
            .SingleAsync(entry => entry.OwnerId == _persistenceOwner.Identifier && entry.EntryId == serializedEvent.Id,
                TestContext.Current.CancellationToken);
        Assert.Equal(serializedEvent.Payload, persistedEntry.Payload);
        InboxEntry persistedDecoy = await _dbContext.Set<InboxEntry>().AsNoTracking()
            .SingleAsync(entry => entry.OwnerId == decoy.OwnerId && entry.EntryId == decoy.EntryId,
                TestContext.Current.CancellationToken);
        Assert.Equal(decoy.Payload, persistedDecoy.Payload);
        Assert.Equal(2, _inboxActivityNotificationCount);
        }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task Write_WithMatchingEntryAndSameEntryIdOnlyDecoy_TreatsReceiptAsSuccessful(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        SerializedEvent serializedEvent = CreateSerializedEvent();
        await _sut.Write(serializedEvent, TestContext.Current.CancellationToken);
        await using var retryDbContext = new TestDbContext(_dbContextOptions);
        InboxEntry decoy = CreateDecoyEntry(serializedEvent, sameOwner: false);
        retryDbContext.Set<InboxEntry>().Add(decoy);
        await retryDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        retryDbContext.ChangeTracker.Clear();
        InboxWriter retryWriter = new InboxWriter(_inboxActivity, retryDbContext, _persistenceOwner);

        // Act
        await retryWriter.Write(serializedEvent, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, await CountPersistedEntries());
        Assert.Empty(retryDbContext.ChangeTracker.Entries());
        InboxEntry persistedEntry = await _dbContext.Set<InboxEntry>().AsNoTracking()
            .SingleAsync(entry => entry.OwnerId == _persistenceOwner.Identifier && entry.EntryId == serializedEvent.Id,
                TestContext.Current.CancellationToken);
        Assert.Equal(serializedEvent.Payload, persistedEntry.Payload);
        InboxEntry persistedDecoy = await _dbContext.Set<InboxEntry>().AsNoTracking()
            .SingleAsync(entry => entry.OwnerId == decoy.OwnerId && entry.EntryId == decoy.EntryId,
                TestContext.Current.CancellationToken);
        Assert.Equal(decoy.Payload, persistedDecoy.Payload);
        Assert.Equal(2, _inboxActivityNotificationCount);
        }

    private FailingDbContext CreateFailingDbContext()
    {
        DbContextOptions<FailingDbContext> options = Options<FailingDbContext>()
            .Options;
        return new FailingDbContext(options);
    }

    private InboxEntry CreateDecoyEntry(SerializedEvent serializedEvent, bool sameOwner)
    {
        return new InboxEntry
        {
            OwnerId = sameOwner ? _persistenceOwner.Identifier : new string('0', 32),
            EntryId = sameOwner ? Guid.NewGuid() : serializedEvent.Id,
            PublishId = serializedEvent.PublishId,
            Handler = serializedEvent.HandlerId.ToString(),
            Event = serializedEvent.EventId.ToString(),
            Payload = "decoy payload"
        };
    }

    private async Task<InboxEntry> LoadEntry(Guid entryId)
    {
        return await _dbContext.Set<InboxEntry>()
            .AsNoTracking()
            .SingleAsync(entry => entry.EntryId == entryId);
    }

    private async Task<int> CountPersistedEntries()
    {
        await using var verificationDbContext = new TestDbContext(_dbContextOptions);
        return await verificationDbContext.Set<InboxEntry>().AsNoTracking().CountAsync();
    }

    private static SerializedEvent CreateSerializedEvent(
        Guid? id = null,
        Guid? publishId = null,
        string payload = "{}")
    {
        return new SerializedEvent(
            id ?? Guid.NewGuid(),
            publishId ?? Guid.NewGuid(),
            EventIdentity.Create<TestEvent>(),
            EventHandlerIdentity.Parse("test-handler"),
            payload);
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
