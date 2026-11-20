using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Operations;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.IntegrationTests.Common.Operations;

public abstract class DeleteOutboxEntriesCommandTests(IntegrationTestsDatabaseBase database) : OutboxOperationsTestsBase(database)
{
    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithOldProcessedEntries_DeletesEntriesForOwner()
    {
        // Arrange

        // To clean
        OutboxEntry old = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1));
        OutboxEntry older = CreateEntry(_owner.Identifier, s_utcNow.AddDays(-2));
        OutboxEntry cleanedPayload = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), payload: "{}");

        // Not to clean
        OutboxEntry atCutoff = CreateEntry(_owner.Identifier, s_utcNow);
        OutboxEntry recentUpdate = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(1));
        recentUpdate.Created = s_utcNow.AddDays(-30);
        OutboxEntry newEntry = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.New);
        OutboxEntry processing = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.InProcessing);
        OutboxEntry failed = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.Failed);
        OutboxEntry otherOwner = CreateEntry("another-owner", s_utcNow.AddMinutes(-1), id: old.EntryId);

        await SeedAsync(old, older, cleanedPayload, atCutoff, recentUpdate, newEntry, processing, failed, otherOwner);

        // Act
        int deleted = await DeleteOutboxEntriesCommand.ExecuteAsync(_dbContext, _owner, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(3, deleted);
        Assert.Equal(0, await DeleteOutboxEntriesCommand.ExecuteAsync(_dbContext, _owner, s_utcNow, TestContext.Current.CancellationToken));

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
}
