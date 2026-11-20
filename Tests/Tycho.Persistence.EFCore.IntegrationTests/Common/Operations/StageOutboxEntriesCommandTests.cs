using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Operations;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.IntegrationTests.Common.Operations;

public abstract class StageOutboxEntriesCommandTests(IntegrationTestsDatabaseBase database) : OutboxOperationsTestsBase(database)
{
    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithEmptyBatch_DoesNotTrackEntries()
    {
        // Act
        await StageOutboxEntriesCommand.ExecuteAsync(_dbContext, [], TestContext.Current.CancellationToken);

        // Assert
        Assert.Empty(_dbContext.ChangeTracker.Entries());
        Assert.Equal(0, await _dbContext.Set<OutboxEntry>().CountAsync(TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithBatch_StagesEntriesWithoutSaving()
    {
        // Arrange
        OutboxEntry[] entries = [CreateEntry(), CreateEntry()];

        // Act
        await StageOutboxEntriesCommand.ExecuteAsync(_dbContext, entries, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(entries.Length, _dbContext.ChangeTracker.Entries<OutboxEntry>().Count());
        Assert.All(_dbContext.ChangeTracker.Entries<OutboxEntry>(), entry => Assert.Equal(EntityState.Added, entry.State));
        await using var verification = new TestDbContext(Database.CreateOptionsBuilder<TestDbContext>().Options);
        Assert.Equal(0, await verification.Set<OutboxEntry>().CountAsync(TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_FollowedBySaveChanges_PersistsAllEntryFields()
    {
        // Arrange
        OutboxEntry[] entries = [CreateEntry(), CreateEntry()];

        // Act
        await StageOutboxEntriesCommand.ExecuteAsync(_dbContext, entries, TestContext.Current.CancellationToken);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        await using var verification = new TestDbContext(Database.CreateOptionsBuilder<TestDbContext>().Options);
        Assert.Equal(entries.Length, await verification.Set<OutboxEntry>().CountAsync(TestContext.Current.CancellationToken));
        foreach (OutboxEntry entry in entries)
        {
            OutboxEntry persisted = await verification.Set<OutboxEntry>().AsNoTracking()
                .SingleAsync(candidate => candidate.EntryId == entry.EntryId, TestContext.Current.CancellationToken);
            Assert.Equal(entry.OwnerId, persisted.OwnerId);
            Assert.Equal(entry.PublishId, persisted.PublishId);
            Assert.Equal(entry.Destination, persisted.Destination);
            Assert.Equal(entry.Event, persisted.Event);
            Assert.Equal(entry.Handler, persisted.Handler);
            Assert.Equal(entry.Payload, persisted.Payload);
            Assert.Equal(EntryState.New, persisted.State);
        }
    }
}
