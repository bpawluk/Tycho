using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Other;

namespace Tycho.Persistence.EFCore.IntegrationTests.Operations;

public abstract class OutboxOperationFixture(DatabaseServers servers) : DatabaseTest(servers)
{
    private protected static readonly DateTime s_utcNow = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private protected readonly PersistenceOwner _owner = new(PersistenceTestInternals.Create(typeof(PersistenceOwner)));
    private protected TestDbContext _dbContext = null!;

    private protected async Task SetupAsync(DatabaseProvider provider)
    {
        await InitializeDatabaseAsync(provider);
        _dbContext = new TestDbContext(Options<TestDbContext>().Options);
        await _dbContext.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
    }

    private protected OutboxEntry CreateEntry() => new()
    {
        OwnerId = _owner.Identifier,
        EntryId = Guid.NewGuid(),
        PublishId = Guid.NewGuid(),
        Event = "event",
        Handler = "handler",
        Payload = "{\"value\":1}",
        Destination = "destination",
        Created = s_utcNow.AddDays(-1),
        Updated = s_utcNow.AddMinutes(-1)
    };

    private protected ClaimRequest CreateClaimRequest() => new(Guid.NewGuid(), s_utcNow, s_utcNow.AddMinutes(5), 3);

    private protected async Task SeedAsync(params OutboxEntry[] entries)
    {
        _dbContext.AddRange(entries);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        _dbContext.ChangeTracker.Clear();
    }

    private protected Task<OutboxEntry> ReadAsync(Guid id) => _dbContext.Set<OutboxEntry>().AsNoTracking().SingleAsync(e => e.OwnerId == _owner.Identifier && e.EntryId == id, TestContext.Current.CancellationToken);

    private protected async Task<List<OutboxEntry>> LoadEntries() => await _dbContext
        .Set<OutboxEntry>()
        .AsNoTracking()
        .ToListAsync(TestContext.Current.CancellationToken);

    private protected static OutboxEntry FindEntry(IEnumerable<OutboxEntry> entries, OutboxEntry expected) =>
        Assert.Single(entries, entry => entry.OwnerId == expected.OwnerId && entry.EntryId == expected.EntryId);

    private protected static OutboxEntry CreateEntry(
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

    private protected static void AssertEntryUnchanged(OutboxEntry expected, OutboxEntry actual)
    {
        Assert.Equal(expected.State, actual.State);
        Assert.Equal(expected.Attempts, actual.Attempts);
        Assert.Equal(expected.ClaimId, actual.ClaimId);
        Assert.Equal(expected.ClaimExpiration, actual.ClaimExpiration);
        Assert.Equal(expected.Updated, actual.Updated);
        Assert.Equal(expected.Created, actual.Created);
        Assert.Equal(expected.PublishId, actual.PublishId);
        Assert.Equal(expected.Payload, actual.Payload);
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            if (_dbContext is not null)
                await _dbContext.DisposeAsync();
        }
        finally
        {
            await base.DisposeAsync();
        }
    }

    private protected sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
