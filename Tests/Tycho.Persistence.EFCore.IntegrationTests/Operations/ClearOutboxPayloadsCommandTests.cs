using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Operations;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Other;

namespace Tycho.Persistence.EFCore.IntegrationTests.Operations;

public sealed class ClearOutboxPayloadsCommandTests(DatabaseServers servers) : OutboxOperationFixture(servers)
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
        OutboxEntry old = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1));
        OutboxEntry older = CreateEntry(_owner.Identifier, s_utcNow.AddDays(-2));

        // Not to clean
        OutboxEntry atCutoff = CreateEntry(_owner.Identifier, s_utcNow);
        OutboxEntry recentUpdate = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(1));
        recentUpdate.Created = s_utcNow.AddDays(-30);
        OutboxEntry newEntry = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.New);
        OutboxEntry processing = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.InProcessing);
        OutboxEntry failed = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), EntryState.Failed);
        OutboxEntry alreadyCleared = CreateEntry(_owner.Identifier, s_utcNow.AddMinutes(-1), payload: "{}");
        OutboxEntry otherOwner = CreateEntry("another-owner", s_utcNow.AddMinutes(-1), id: old.EntryId);

        await SeedAsync(old, older, atCutoff, recentUpdate, newEntry, processing, failed, alreadyCleared, otherOwner);

        // Act
        int cleaned = await ClearOutboxPayloadsCommand.ExecuteAsync(_dbContext, _owner, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, cleaned);
        Assert.Equal(0, await ClearOutboxPayloadsCommand.ExecuteAsync(_dbContext, _owner, s_utcNow, TestContext.Current.CancellationToken));

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
}
