using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Operations;

namespace Tycho.Persistence.EFCore.IntegrationTests.Common.Operations;

public abstract class DeleteInboxEntriesCommandTests(IntegrationTestsDatabaseBase database) : InboxOperationsTestsBase(database)
{
    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithOldProcessedEntries_DeletesEntriesForOwner()
    {
        // Arrange

        // To clean
        InboxEntry old = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1));
        InboxEntry older = CreateEntry(_owner.Identifier, s_utcNow.AddDays(-2));
        InboxEntry cleanedPayload = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), payload: "{}");

        // Not to clean
        InboxEntry atCutoff = CreateEntry(_owner.Identifier, s_utcNow);
        InboxEntry recentUpdate = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(1));
        recentUpdate.Created = s_utcNow.AddDays(-30);
        InboxEntry newEntry = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.New);
        InboxEntry processing = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.InProcessing);
        InboxEntry failed = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.Failed);
        InboxEntry otherOwner = CreateEntry("another-owner", s_utcNow.AddMinutes(-1), id: old.EntryId);

        await SeedAsync(old, older, cleanedPayload, atCutoff, recentUpdate, newEntry, processing, failed, otherOwner);

        // Act
        int deleted = await DeleteInboxEntriesCommand.ExecuteAsync(_dbContext, _owner, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(3, deleted);
        Assert.Equal(0, await DeleteInboxEntriesCommand.ExecuteAsync(_dbContext, _owner, s_utcNow, TestContext.Current.CancellationToken));

        List<InboxEntry> entries = await LoadEntries();
        Assert.Equal(6, entries.Count);

        foreach (InboxEntry removed in new[] { old, older, cleanedPayload })
        {
            Assert.DoesNotContain(entries, entry => entry.OwnerId == removed.OwnerId && entry.EntryId == removed.EntryId);
        }

        foreach (InboxEntry retained in new[] { atCutoff, recentUpdate, newEntry, processing, failed, otherOwner })
        {
            Assert.Equal(retained.Payload, FindEntry(entries, retained).Payload);
        }
    }
}
