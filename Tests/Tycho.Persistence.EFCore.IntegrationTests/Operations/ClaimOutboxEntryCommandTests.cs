using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Operations;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Other;

namespace Tycho.Persistence.EFCore.IntegrationTests.Operations;

public sealed class ClaimOutboxEntryCommandTests(DatabaseServers servers) : OutboxOperationFixture(servers)
{
    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithNewEntry_ClaimsEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry();
        entry.State = EntryState.New;
        entry.Attempts = 0;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = CreateClaimRequest();

        // Act
        ClaimResult<OutboxEntry> result = await ClaimOutboxEntryCommand.ExecuteAsync(
            _dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.Claimed, result.Status);
        Assert.NotNull(result.Entry);
        Assert.Equal(entry.EntryId, result.Entry.EntryId);
        Assert.Equal(entry.Payload, result.Entry.Payload);
        OutboxEntry persisted = await ReadAsync(entry.EntryId);
        Assert.Equal(EntryState.InProcessing, persisted.State);
        Assert.Equal(1u, persisted.Attempts);
        Assert.Equal(request.ClaimId, persisted.ClaimId);
        Assert.Equal(request.Expiration, persisted.ClaimExpiration);
        Assert.Equal(s_utcNow, persisted.Updated);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithFailedEntryBelowAttemptLimit_ClaimsEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry();
        entry.State = EntryState.Failed;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = CreateClaimRequest();

        // Act
        ClaimResult<OutboxEntry> result = await ClaimOutboxEntryCommand.ExecuteAsync(
            _dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.Claimed, result.Status);
        Assert.NotNull(result.Entry);
        Assert.Equal(entry.EntryId, result.Entry.EntryId);
        Assert.Equal(entry.Payload, result.Entry.Payload);
        OutboxEntry persisted = await ReadAsync(entry.EntryId);
        Assert.Equal(EntryState.InProcessing, persisted.State);
        Assert.Equal(3u, persisted.Attempts);
        Assert.Equal(request.ClaimId, persisted.ClaimId);
        Assert.Equal(request.Expiration, persisted.ClaimExpiration);
        Assert.Equal(s_utcNow, persisted.Updated);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithFailedEntryAtAttemptLimit_DoesNotClaimEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry();
        entry.State = EntryState.Failed;
        entry.Attempts = 3;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = CreateClaimRequest();

        // Act
        ClaimResult<OutboxEntry> result = await ClaimOutboxEntryCommand.ExecuteAsync(
            _dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.NotClaimed, result.Status);
        Assert.Null(result.Entry);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithExpiredClaimBelowAttemptLimit_ClaimsEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = CreateClaimRequest();

        // Act
        ClaimResult<OutboxEntry> result = await ClaimOutboxEntryCommand.ExecuteAsync(
            _dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.Claimed, result.Status);
        Assert.NotNull(result.Entry);
        Assert.Equal(entry.EntryId, result.Entry.EntryId);
        Assert.Equal(entry.Payload, result.Entry.Payload);
        OutboxEntry persisted = await ReadAsync(entry.EntryId);
        Assert.Equal(EntryState.InProcessing, persisted.State);
        Assert.Equal(3u, persisted.Attempts);
        Assert.Equal(request.ClaimId, persisted.ClaimId);
        Assert.Equal(request.Expiration, persisted.ClaimExpiration);
        Assert.Equal(s_utcNow, persisted.Updated);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithClaimExpiringAtCurrentTime_DoesNotClaimEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(0);
        await SeedAsync(entry);
        ClaimRequest request = CreateClaimRequest();

        // Act
        ClaimResult<OutboxEntry> result = await ClaimOutboxEntryCommand.ExecuteAsync(
            _dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.NotClaimed, result.Status);
        Assert.Null(result.Entry);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithActiveClaim_DoesNotClaimEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(1);
        await SeedAsync(entry);
        ClaimRequest request = CreateClaimRequest();

        // Act
        ClaimResult<OutboxEntry> result = await ClaimOutboxEntryCommand.ExecuteAsync(
            _dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.NotClaimed, result.Status);
        Assert.Null(result.Entry);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithExpiredClaimAtAttemptLimit_DoesNotClaimEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 3;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = CreateClaimRequest();

        // Act
        ClaimResult<OutboxEntry> result = await ClaimOutboxEntryCommand.ExecuteAsync(
            _dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.NotClaimed, result.Status);
        Assert.Null(result.Entry);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithProcessedEntry_DoesNotClaimEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry();
        entry.State = EntryState.Processed;
        entry.Attempts = 0;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = CreateClaimRequest();

        // Act
        ClaimResult<OutboxEntry> result = await ClaimOutboxEntryCommand.ExecuteAsync(
            _dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.NotClaimed, result.Status);
        Assert.Null(result.Entry);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithMultipleOwnersAndTiedTimestamps_ClaimsOnlyOwnersEntriesInOrder(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry oldest = CreateEntry();
        oldest.EntryId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        OutboxEntry tied = CreateEntry();
        tied.EntryId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        OutboxEntry newer = CreateEntry();
        newer.Updated = oldest.Updated.AddSeconds(1);
        OutboxEntry otherOwner = CreateEntry();
        otherOwner.OwnerId = new string('0', 32);
        otherOwner.Updated = oldest.Updated.AddDays(-1);
        await SeedAsync(newer, tied, oldest, otherOwner);

        // Act
        var results = new List<ClaimResult<OutboxEntry>>();
        for (int i = 0; i < 4; i++)
        {
            results.Add(await ClaimOutboxEntryCommand.ExecuteAsync(
                _dbContext, _owner, CreateClaimRequest(), TestContext.Current.CancellationToken));
        }

        // Assert
        Assert.Equal(new[] { oldest.EntryId, tied.EntryId, newer.EntryId }, results.Take(3).Select(result => result.Entry!.EntryId));
        Assert.Equal(ClaimStatus.NotClaimed, results[3].Status);
        Assert.Equal(EntryState.New, (await _dbContext.Set<OutboxEntry>().AsNoTracking().SingleAsync(e => e.OwnerId == otherOwner.OwnerId, TestContext.Current.CancellationToken)).State);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_WithCompetingContexts_ClaimsEntryOnlyOnce(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry();
        await SeedAsync(entry);
        await using var first = new TestDbContext(Options<TestDbContext>().Options);
        await using var second = new TestDbContext(Options<TestDbContext>().Options);
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        async Task<ClaimResult<OutboxEntry>> ClaimAsync(TestDbContext context)
        {
            await start.Task;
            return await ClaimOutboxEntryCommand.ExecuteAsync(context, _owner, CreateClaimRequest(), TestContext.Current.CancellationToken);
        }
        Task<ClaimResult<OutboxEntry>> firstClaim = ClaimAsync(first);
        Task<ClaimResult<OutboxEntry>> secondClaim = ClaimAsync(second);

        // Act
        start.SetResult();
        ClaimResult<OutboxEntry>[] results = await Task.WhenAll(firstClaim, secondClaim);

        // Assert
        Assert.Single(results, r => r.Status == ClaimStatus.Claimed);
        Assert.Single(results, r => r.Status == ClaimStatus.NotClaimed);
        Assert.Equal(1u, (await ReadAsync(entry.EntryId)).Attempts);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task ExecuteAsync_InsideRetryingExecutionStrategy_RejectsWithoutChangingEntry(DatabaseProvider provider)
    {
        // Arrange
        await SetupAsync(provider);
        OutboxEntry entry = CreateEntry();
        await SeedAsync(entry);
        await using var context = new TestDbContext(Options<TestDbContext>()
            .ReplaceService<IExecutionStrategyFactory, RetryingExecutionStrategyFactory>().Options);
        IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

        // Act
        Task Act() => strategy.ExecuteAsync(() =>
            ClaimOutboxEntryCommand.ExecuteAsync(context, _owner, CreateClaimRequest(), TestContext.Current.CancellationToken));

        // Assert
        Assert.True(strategy.RetriesOnFailure);
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Equal(EntryState.New, (await ReadAsync(entry.EntryId)).State);
    }
}
