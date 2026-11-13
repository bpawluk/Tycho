using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Operations;
using Tycho.Persistence.EFCore.TestUtils.Databases;

namespace Tycho.Persistence.EFCore.IntegrationTests.Operations;

public sealed class FindInboxEntryQueryTests(DatabaseServers servers) : InboxOperationFixture(servers)
{
    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithMatchingOwnerAndEntryId_ReturnsUntrackedEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();
        InboxEntry decoy = CreateEntry();
        decoy.EntryId = entry.EntryId;
        decoy.OwnerId = new string('0', 32);
        await SeedAsync(decoy, entry);

        // Act
        InboxEntry? found = await FindInboxEntryQuery.ExecuteAsync(_dbContext, _owner, entry.EntryId, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(entry.PublishId, found.PublishId);
        Assert.Equal(entry.Payload, found.Payload);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithMissingEntry_ReturnsNull(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        await SeedAsync(CreateEntry());
        Guid missingEntryId = Guid.NewGuid();

        // Act
        InboxEntry? found = await FindInboxEntryQuery.ExecuteAsync(_dbContext, _owner, missingEntryId, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(found);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithEntryOwnedByAnotherOwner_ReturnsNull(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();
        entry.OwnerId = new string('0', 32);
        await SeedAsync(entry);

        // Act
        InboxEntry? found = await FindInboxEntryQuery.ExecuteAsync(_dbContext, _owner, entry.EntryId, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(found);
    }
}
