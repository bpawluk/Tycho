using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Operations;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Other;

namespace Tycho.Persistence.EFCore.IntegrationTests.Operations;

public sealed class ClearInboxPayloadsCommandTests(DatabaseServers servers) : InboxOperationFixture(servers)
{
    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithOldProcessedEntries_ClearsPayloadsForOwner(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);

        // To clean
        InboxEntry old = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1));
        InboxEntry older = CreateEntry(_owner.Identifier, s_utcNow.AddDays(-2));

        // Not to clean
        InboxEntry atCutoff = CreateEntry(_owner.Identifier, s_utcNow);
        InboxEntry recentUpdate = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(1));
        recentUpdate.Created = s_utcNow.AddDays(-30);
        InboxEntry newEntry = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.New);
        InboxEntry processing = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.InProcessing);
        InboxEntry failed = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.Failed);
        InboxEntry alreadyCleared = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), payload: "{}");
        InboxEntry otherOwner = CreateEntry("another-owner", s_utcNow.AddMinutes(-1), id: old.EntryId);

        await SeedAsync(old, older, atCutoff, recentUpdate, newEntry, processing, failed, alreadyCleared, otherOwner);

        // Act
        int cleaned = await ClearInboxPayloadsCommand.ExecuteAsync(_dbContext, _owner, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, cleaned);
        Assert.Equal(0, await ClearInboxPayloadsCommand.ExecuteAsync(_dbContext, _owner, s_utcNow, TestContext.Current.CancellationToken));

        List<InboxEntry> entries = await LoadEntries();
        Assert.Equal(9, entries.Count);

        Assert.Equal("{}", FindEntry(entries, old).Payload);
        Assert.Equal("{}", FindEntry(entries, older).Payload);
        Assert.Equal("{}", FindEntry(entries, alreadyCleared).Payload);

        foreach (InboxEntry unchanged in new[] { atCutoff, recentUpdate, newEntry, processing, failed, otherOwner })
        {
            Assert.Equal(unchanged.Payload, FindEntry(entries, unchanged).Payload);
        }

        InboxEntry persisted = FindEntry(entries, old);
        Assert.Equal(old.Updated, persisted.Updated);
        Assert.Equal(old.State, persisted.State);
        Assert.Equal(old.PublishId, persisted.PublishId);
    }
}
