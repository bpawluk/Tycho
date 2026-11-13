using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Operations;
using Tycho.Persistence.EFCore.TestUtils.Databases;

namespace Tycho.Persistence.EFCore.IntegrationTests.Operations;

public sealed class InsertInboxEntryCommandTests(DatabaseServers servers) : InboxOperationFixture(servers)
{
    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithNewEntry_PersistsEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();

        // Act
        await InsertInboxEntryCommand.ExecuteAsync(_dbContext, entry, TestContext.Current.CancellationToken);

        // Assert
        InboxEntry persisted = await ReadAsync(entry.EntryId);
        Assert.Equal(entry.PublishId, persisted.PublishId);
        Assert.Equal(entry.Event, persisted.Event);
        Assert.Equal(entry.Handler, persisted.Handler);
        Assert.Equal(entry.Payload, persisted.Payload);
        Assert.Equal(EntryState.New, persisted.State);
        Assert.Equal(0u, persisted.Attempts);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithDuplicateEntry_DetachesFailedInsert(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry existing = CreateEntry();
        await SeedAsync(existing);
        InboxEntry duplicate = CreateEntry();
        duplicate.EntryId = existing.EntryId;

        // Act
        Task Act() => InsertInboxEntryCommand.ExecuteAsync(_dbContext, duplicate, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(Act);
        Assert.Equal(EntityState.Detached, _dbContext.Entry(duplicate).State);
        Assert.Equal(1, await _dbContext.Set<InboxEntry>().CountAsync(TestContext.Current.CancellationToken));
        AssertEntryUnchanged(existing, await ReadAsync(existing.EntryId));
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_AfterDuplicateFailure_AllowsSubsequentWrites(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry existing = CreateEntry();
        await SeedAsync(existing);
        InboxEntry duplicate = CreateEntry();
        duplicate.EntryId = existing.EntryId;
        await Assert.ThrowsAsync<DbUpdateException>(() =>
            InsertInboxEntryCommand.ExecuteAsync(_dbContext, duplicate, TestContext.Current.CancellationToken));
        InboxEntry next = CreateEntry();

        // Act
        await InsertInboxEntryCommand.ExecuteAsync(_dbContext, next, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, await _dbContext.Set<InboxEntry>().CountAsync(TestContext.Current.CancellationToken));
        AssertEntryUnchanged(next, await ReadAsync(next.EntryId));
    }
}
