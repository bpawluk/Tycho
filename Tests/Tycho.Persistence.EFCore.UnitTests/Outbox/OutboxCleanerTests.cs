using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Common;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.UnitTests._Utils;

namespace Tycho.Persistence.EFCore.UnitTests.Outbox;

public sealed class OutboxCleanerTests : IAsyncLifetime
{
    private static readonly DateTime s_cutoff = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private SqliteConnection _connection = default!;
    private TestDbContext _dbContext = default!;
    private PersistenceOwner _owner = default!;
    private OutboxCleaner _sut = default!;

    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync(TestContext.Current.CancellationToken);

        DbContextOptions<TestDbContext> options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;
        _dbContext = new TestDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        _owner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
        _sut = new OutboxCleaner(_dbContext, _owner);
    }

    [Fact]
    public async Task CleanPayloadsAsync_ClearsOldProcessedEntriesForOwner()
    {
        // Arrange

        // To clean
        OutboxEntry old = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(-1));
        OutboxEntry older = CreateEntry(_owner.Identifier, s_cutoff.AddDays(-2));

        // Not to clean
        OutboxEntry atCutoff = CreateEntry(_owner.Identifier, s_cutoff);
        OutboxEntry recentUpdate = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(1));
        recentUpdate.Created = s_cutoff.AddDays(-30);
        OutboxEntry newEntry = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(-1), EntryState.New);
        OutboxEntry processing = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(-1), EntryState.InProcessing);
        OutboxEntry failed = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(-1), EntryState.Failed);
        OutboxEntry alreadyCleared = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(-1), payload: "{}");
        OutboxEntry otherOwner = CreateEntry("another-owner", s_cutoff.AddMinutes(-1), id: old.EntryId);

        await SeedEntries(old, older, atCutoff, recentUpdate, newEntry, processing, failed, alreadyCleared, otherOwner);

        // Act
        int cleaned = await _sut.CleanPayloadsAsync(s_cutoff, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, cleaned);

        List<OutboxEntry> entries = await LoadEntries();
        Assert.Equal(9, entries.Count);

        Assert.Equal("{}", FindEntry(entries, old).Payload);
        Assert.Equal("{}", FindEntry(entries, older).Payload);
        Assert.Equal("{}", FindEntry(entries, alreadyCleared).Payload);

        foreach (OutboxEntry unchanged in new[] { atCutoff, recentUpdate, newEntry, processing, failed, otherOwner })
        {
            Assert.Equal(unchanged.Payload, FindEntry(entries, unchanged).Payload);
        }

        OutboxEntry persisted = FindEntry(entries, old);
        Assert.Equal(old.Updated, persisted.Updated);
        Assert.Equal(old.State, persisted.State);
        Assert.Equal(old.PublishId, persisted.PublishId);
        Assert.Equal(old.Destination, persisted.Destination);
    }

    [Fact]
    public async Task CleanEntriesAsync_DeletesOldProcessedEntriesForOwner()
    {
        // Arrange

        // To clean
        OutboxEntry old = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(-1));
        OutboxEntry older = CreateEntry(_owner.Identifier, s_cutoff.AddDays(-2));
        OutboxEntry cleanedPayload = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(-1), payload: "{}");

        // Not to clean
        OutboxEntry atCutoff = CreateEntry(_owner.Identifier, s_cutoff);
        OutboxEntry recentUpdate = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(1));
        recentUpdate.Created = s_cutoff.AddDays(-30);
        OutboxEntry newEntry = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(-1), EntryState.New);
        OutboxEntry processing = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(-1), EntryState.InProcessing);
        OutboxEntry failed = CreateEntry(_owner.Identifier, s_cutoff.AddMinutes(-1), EntryState.Failed);
        OutboxEntry otherOwner = CreateEntry("another-owner", s_cutoff.AddMinutes(-1), id: old.EntryId);

        await SeedEntries(old, older, cleanedPayload, atCutoff, recentUpdate, newEntry, processing, failed, otherOwner);

        // Act
        int deleted = await _sut.CleanEntriesAsync(s_cutoff, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(3, deleted);

        List<OutboxEntry> entries = await LoadEntries();
        Assert.Equal(6, entries.Count);

        foreach (OutboxEntry removed in new[] { old, older, cleanedPayload })
        {
            Assert.DoesNotContain(entries, entry => entry.OwnerId == removed.OwnerId && entry.EntryId == removed.EntryId);
        }

        foreach (OutboxEntry retained in new[] { atCutoff, recentUpdate, newEntry, processing, failed, otherOwner })
        {
            Assert.Equal(retained.Payload, FindEntry(entries, retained).Payload);
        }
    }

    private async Task SeedEntries(params OutboxEntry[] entries)
    {
        _dbContext.Set<OutboxEntry>().AddRange(entries);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private async Task<List<OutboxEntry>> LoadEntries() => await _dbContext
        .Set<OutboxEntry>()
        .AsNoTracking()
        .ToListAsync(TestContext.Current.CancellationToken);

    private static OutboxEntry FindEntry(IEnumerable<OutboxEntry> entries, OutboxEntry expected) =>
        Assert.Single(entries, entry => entry.OwnerId == expected.OwnerId && entry.EntryId == expected.EntryId);

    private static OutboxEntry CreateEntry(
        string ownerId,
        DateTime updated,
        EntryState state = EntryState.Processed,
        string payload = "original payload",
        Guid? id = null) => new()
        {
            OwnerId = ownerId,
            EntryId = id ?? Guid.NewGuid(),
            PublishId = Guid.NewGuid(),
            Destination = "test-route",
            Handler = "TestHandler",
            Event = "TestEvent",
            Payload = payload,
            State = state,
            Created = updated.AddDays(-1),
            Updated = updated
        };

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
