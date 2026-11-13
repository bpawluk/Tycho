using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Operations;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Other;

namespace Tycho.Persistence.EFCore.IntegrationTests.Operations;

public sealed class FailInboxClaimCommandTests(DatabaseServers servers) : InboxOperationFixture(servers)
{
    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithValidClaim_UpdatesEntryAndClearsClaim(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddMinutes(5);
        PersistenceOwner owner = _owner;
        Guid entryId = entry.EntryId;
        Guid claimId = entry.ClaimId;
        await SeedAsync(entry);

        // Act
        int affected = await FailInboxClaimCommand.ExecuteAsync(
            _dbContext, owner, entryId, claimId, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, affected);
        InboxEntry persisted = await ReadAsync(entry.EntryId);
        Assert.Equal(EntryState.Failed, persisted.State);
        Assert.Equal(Guid.Empty, persisted.ClaimId);
        Assert.Equal(DateTime.MinValue, persisted.ClaimExpiration);
        Assert.Equal(s_utcNow, persisted.Updated);
        Assert.Equal(entry.Attempts, persisted.Attempts);
        Assert.Equal(entry.Payload, persisted.Payload);
        Assert.Equal(entry.Created, persisted.Created);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithWrongOwner_DoesNotModifyEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddMinutes(5);
        PersistenceOwner owner = _owner;
        Guid entryId = entry.EntryId;
        Guid claimId = entry.ClaimId;
        owner = new PersistenceOwner(PersistenceTestInternals.Create(typeof(string)));
        await SeedAsync(entry);

        // Act
        int affected = await FailInboxClaimCommand.ExecuteAsync(
            _dbContext, owner, entryId, claimId, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, affected);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithWrongEntryId_DoesNotModifyEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddMinutes(5);
        PersistenceOwner owner = _owner;
        Guid entryId = entry.EntryId;
        Guid claimId = entry.ClaimId;
        entryId = Guid.NewGuid();
        await SeedAsync(entry);

        // Act
        int affected = await FailInboxClaimCommand.ExecuteAsync(
            _dbContext, owner, entryId, claimId, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, affected);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithWrongClaimId_DoesNotModifyEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddMinutes(5);
        PersistenceOwner owner = _owner;
        Guid entryId = entry.EntryId;
        Guid claimId = entry.ClaimId;
        claimId = Guid.NewGuid();
        await SeedAsync(entry);

        // Act
        int affected = await FailInboxClaimCommand.ExecuteAsync(
            _dbContext, owner, entryId, claimId, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, affected);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithFailedEntry_DoesNotModifyEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddMinutes(5);
        PersistenceOwner owner = _owner;
        Guid entryId = entry.EntryId;
        Guid claimId = entry.ClaimId;
        entry.State = EntryState.Failed;
        await SeedAsync(entry);

        // Act
        int affected = await FailInboxClaimCommand.ExecuteAsync(
            _dbContext, owner, entryId, claimId, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, affected);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithNewEntry_DoesNotModifyEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddMinutes(5);
        PersistenceOwner owner = _owner;
        Guid entryId = entry.EntryId;
        Guid claimId = entry.ClaimId;
        entry.State = EntryState.New;
        await SeedAsync(entry);

        // Act
        int affected = await FailInboxClaimCommand.ExecuteAsync(
            _dbContext, owner, entryId, claimId, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, affected);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithProcessedEntry_DoesNotModifyEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddMinutes(5);
        PersistenceOwner owner = _owner;
        Guid entryId = entry.EntryId;
        Guid claimId = entry.ClaimId;
        entry.State = EntryState.Processed;
        await SeedAsync(entry);

        // Act
        int affected = await FailInboxClaimCommand.ExecuteAsync(
            _dbContext, owner, entryId, claimId, s_utcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, affected);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_AfterReclaim_RejectsPreviousHolder(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        InboxEntry entry = CreateEntry();
        await SeedAsync(entry);
        ClaimRequest first = CreateClaimRequest();
        await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, first, TestContext.Current.CancellationToken);
        ClaimRequest next = new(Guid.NewGuid(), first.Expiration.AddSeconds(1), first.Expiration.AddMinutes(5), 3);
        ClaimResult<InboxEntry> result = await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, next, TestContext.Current.CancellationToken);

        // Act
        int affected = await FailInboxClaimCommand.ExecuteAsync(_dbContext, _owner, entry.EntryId, first.ClaimId, next.UtcNow, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(next.ClaimId, result.Entry?.ClaimId);
        Assert.Equal(0, affected);
        Assert.Equal(next.ClaimId, (await ReadAsync(entry.EntryId)).ClaimId);
    }
}
