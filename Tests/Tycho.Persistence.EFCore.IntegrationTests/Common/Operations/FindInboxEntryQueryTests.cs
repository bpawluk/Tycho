using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Operations;

namespace Tycho.Persistence.EFCore.IntegrationTests.Common.Operations;

public abstract class FindInboxEntryQueryTests(IntegrationTestsDatabaseBase database) : InboxOperationsTestsBase(database)
{
    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithMatchingOwnerAndEntryId_ReturnsUntrackedEntry()
    {
        // Arrange
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

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithMissingEntry_ReturnsNull()
    {
        // Arrange
        await SeedAsync(CreateEntry());
        Guid missingEntryId = Guid.NewGuid();

        // Act
        InboxEntry? found = await FindInboxEntryQuery.ExecuteAsync(_dbContext, _owner, missingEntryId, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(found);
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithEntryOwnedByAnotherOwner_ReturnsNull()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        entry.OwnerId = new string('0', 32);
        await SeedAsync(entry);

        // Act
        InboxEntry? found = await FindInboxEntryQuery.ExecuteAsync(_dbContext, _owner, entry.EntryId, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(found);
    }
}
