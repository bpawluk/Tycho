using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Operations;

namespace Tycho.Persistence.EFCore.IntegrationTests.Common.Operations;

public abstract class ClaimInboxEntryCommandTests(IntegrationTestsDatabaseBase database) : InboxOperationsTestsBase(database)
{
    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithNewEntry_ClaimsEntry()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.New;
        entry.Attempts = 0;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = InboxOperationsTestsBase.CreateClaimRequest();

        // Act
        ClaimResult<InboxEntry> result = await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.Claimed, result.Status);
        Assert.NotNull(result.Entry);
        Assert.Equal(entry.EntryId, result.Entry.EntryId);
        Assert.Equal(entry.Payload, result.Entry.Payload);
        InboxEntry persisted = await ReadAsync(entry.EntryId);
        Assert.Equal(EntryState.InProcessing, persisted.State);
        Assert.Equal(1u, persisted.Attempts);
        Assert.Equal(request.ClaimId, persisted.ClaimId);
        Assert.Equal(request.Expiration, persisted.ClaimExpiration);
        Assert.Equal(s_utcNow, persisted.Updated);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithFailedEntryBelowAttemptLimit_ClaimsEntry()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.Failed;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = InboxOperationsTestsBase.CreateClaimRequest();

        // Act
        ClaimResult<InboxEntry> result = await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.Claimed, result.Status);
        Assert.NotNull(result.Entry);
        Assert.Equal(entry.EntryId, result.Entry.EntryId);
        Assert.Equal(entry.Payload, result.Entry.Payload);
        InboxEntry persisted = await ReadAsync(entry.EntryId);
        Assert.Equal(EntryState.InProcessing, persisted.State);
        Assert.Equal(3u, persisted.Attempts);
        Assert.Equal(request.ClaimId, persisted.ClaimId);
        Assert.Equal(request.Expiration, persisted.ClaimExpiration);
        Assert.Equal(s_utcNow, persisted.Updated);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithFailedEntryAtAttemptLimit_DoesNotClaimEntry()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.Failed;
        entry.Attempts = 3;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = InboxOperationsTestsBase.CreateClaimRequest();

        // Act
        ClaimResult<InboxEntry> result = await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.NotClaimed, result.Status);
        Assert.Null(result.Entry);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithExpiredClaimBelowAttemptLimit_ClaimsEntry()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = InboxOperationsTestsBase.CreateClaimRequest();

        // Act
        ClaimResult<InboxEntry> result = await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.Claimed, result.Status);
        Assert.NotNull(result.Entry);
        Assert.Equal(entry.EntryId, result.Entry.EntryId);
        Assert.Equal(entry.Payload, result.Entry.Payload);
        InboxEntry persisted = await ReadAsync(entry.EntryId);
        Assert.Equal(EntryState.InProcessing, persisted.State);
        Assert.Equal(3u, persisted.Attempts);
        Assert.Equal(request.ClaimId, persisted.ClaimId);
        Assert.Equal(request.Expiration, persisted.ClaimExpiration);
        Assert.Equal(s_utcNow, persisted.Updated);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithClaimExpiringAtCurrentTime_DoesNotClaimEntry()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(0);
        await SeedAsync(entry);
        ClaimRequest request = InboxOperationsTestsBase.CreateClaimRequest();

        // Act
        ClaimResult<InboxEntry> result = await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.NotClaimed, result.Status);
        Assert.Null(result.Entry);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithActiveClaim_DoesNotClaimEntry()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 2;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(1);
        await SeedAsync(entry);
        ClaimRequest request = InboxOperationsTestsBase.CreateClaimRequest();

        // Act
        ClaimResult<InboxEntry> result = await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.NotClaimed, result.Status);
        Assert.Null(result.Entry);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithExpiredClaimAtAttemptLimit_DoesNotClaimEntry()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.InProcessing;
        entry.Attempts = 3;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = InboxOperationsTestsBase.CreateClaimRequest();

        // Act
        ClaimResult<InboxEntry> result = await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.NotClaimed, result.Status);
        Assert.Null(result.Entry);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithProcessedEntry_DoesNotClaimEntry()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        entry.State = EntryState.Processed;
        entry.Attempts = 0;
        entry.ClaimId = Guid.NewGuid();
        entry.ClaimExpiration = s_utcNow.AddSeconds(-1);
        await SeedAsync(entry);
        ClaimRequest request = InboxOperationsTestsBase.CreateClaimRequest();

        // Act
        ClaimResult<InboxEntry> result = await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ClaimStatus.NotClaimed, result.Status);
        Assert.Null(result.Entry);
        AssertEntryUnchanged(entry, await ReadAsync(entry.EntryId));
        Assert.Empty(_dbContext.ChangeTracker.Entries());
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithMultipleOwnersAndTiedTimestamps_ClaimsOnlyOwnersEntriesInOrder()
    {
        // Arrange
        InboxEntry oldest = CreateEntry();
        oldest.EntryId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        InboxEntry tied = CreateEntry();
        tied.EntryId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        InboxEntry newer = CreateEntry();
        newer.Updated = oldest.Updated.AddSeconds(1);
        InboxEntry otherOwner = CreateEntry();
        otherOwner.OwnerId = new string('0', 32);
        otherOwner.Updated = oldest.Updated.AddDays(-1);
        await SeedAsync(newer, tied, oldest, otherOwner);

        // Act
        var results = new List<ClaimResult<InboxEntry>>();
        for (int i = 0; i < 4; i++)
        {
            results.Add(await ClaimInboxEntryCommand.ExecuteAsync(_dbContext, _owner, InboxOperationsTestsBase.CreateClaimRequest(), TestContext.Current.CancellationToken));
        }

        // Assert
        Assert.Equal(new[] { oldest.EntryId, tied.EntryId, newer.EntryId }, results.Take(3).Select(result => result.Entry!.EntryId));
        Assert.Equal(ClaimStatus.NotClaimed, results[3].Status);
        Assert.Equal(EntryState.New, (await _dbContext.Set<InboxEntry>().AsNoTracking().SingleAsync(e => e.OwnerId == otherOwner.OwnerId, TestContext.Current.CancellationToken)).State);
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_WithCompetingContexts_ClaimsEntryOnlyOnce()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        await SeedAsync(entry);
        await using var first = new TestDbContext(Database.CreateOptionsBuilder<TestDbContext>().Options);
        await using var second = new TestDbContext(Database.CreateOptionsBuilder<TestDbContext>().Options);
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        async Task<ClaimResult<InboxEntry>> ClaimAsync(TestDbContext context)
        {
            await start.Task;
            return await ClaimInboxEntryCommand.ExecuteAsync(context, _owner, InboxOperationsTestsBase.CreateClaimRequest(), TestContext.Current.CancellationToken);
        }
        Task<ClaimResult<InboxEntry>> firstClaim = ClaimAsync(first);
        Task<ClaimResult<InboxEntry>> secondClaim = ClaimAsync(second);

        // Act
        start.SetResult();
        ClaimResult<InboxEntry>[] results = await Task.WhenAll(firstClaim, secondClaim);

        // Assert
        Assert.Single(results, r => r.Status == ClaimStatus.Claimed);
        Assert.Single(results, r => r.Status == ClaimStatus.NotClaimed);
        Assert.Equal(1u, (await ReadAsync(entry.EntryId)).Attempts);
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_InsideRetryingExecutionStrategy_RejectsWithoutChangingEntry()
    {
        // Arrange
        InboxEntry entry = CreateEntry();
        await SeedAsync(entry);
        await using var context = new TestDbContext(Database.CreateOptionsBuilder<TestDbContext>()
            .ReplaceService<IExecutionStrategyFactory, RetryingExecutionStrategyFactory>().Options);
        IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

        // Act
        Task Act() => strategy.ExecuteAsync(() => ClaimInboxEntryCommand.ExecuteAsync(context, _owner, InboxOperationsTestsBase.CreateClaimRequest(), TestContext.Current.CancellationToken));

        // Assert
        Assert.True(strategy.RetriesOnFailure);
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Equal(EntryState.New, (await ReadAsync(entry.EntryId)).State);
    }
}
