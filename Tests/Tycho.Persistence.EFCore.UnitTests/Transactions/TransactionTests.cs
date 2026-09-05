using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Transactions;

namespace Tycho.Persistence.EFCore.UnitTests.Transactions;

public sealed class TransactionTests : IAsyncLifetime
{
    private DbContextOptions<TestDbContext> _dbContextOptions = default!;
    private TestDbContext _dbContext = default!;
    private SqliteConnection _connection = default!;

    private Transaction _sut = default!;

    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        DbContextOptionsBuilder<TestDbContext> optionsBuilder = new();
        _dbContextOptions = optionsBuilder.UseSqlite(_connection).Options;

        _dbContext = new TestDbContext(_dbContextOptions);
        await _dbContext.Database.EnsureCreatedAsync();

        _sut = new Transaction(_dbContext);
    }

    [Fact]
    public void ExecuteAfterCommit_WithNullAction_ThrowsArgumentNullException()
    {
        // Act
        void act() => _sut.ExecuteAfterCommit(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BeginAsync_WithoutActiveTransaction_SetsIsInProgressTrue()
    {
        // Act
        await _sut.BeginAsync(CancellationToken.None);

        // Assert
        Assert.True(_sut.IsInProgress);
    }

    [Fact]
    public async Task BeginAsync_WithAlreadyActiveTransaction_DoesNotThrowAndKeepsIsInProgressTrue()
    {
        // Arrange
        await _sut.BeginAsync(CancellationToken.None);

        // Act
        await _sut.BeginAsync(CancellationToken.None);

        // Assert
        Assert.True(_sut.IsInProgress);
    }

    [Fact]
    public async Task CommitAsync_WithoutActiveTransaction_DoesNotExecuteCallbacksAndDoesNotPersistChanges()
    {
        // Arrange
        int callbackExecutionCount = 0;
        _sut.ExecuteAfterCommit(() => callbackExecutionCount++);

        _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());

        // Act
        await _sut.CommitAsync(CancellationToken.None);

        // Assert
        Assert.False(_sut.IsInProgress);
        Assert.Equal(0, callbackExecutionCount);
        Assert.Equal(0, await CountPersistedOutboxEntries());
    }

    [Fact]
    public async Task CommitAsync_WithActiveTransaction_PersistsChangesAndExecutesAfterCommitActions()
    {
        // Arrange
        var callbackOrder = new List<int>();

        await _sut.BeginAsync(CancellationToken.None);

        _sut.ExecuteAfterCommit(() => callbackOrder.Add(1));
        _sut.ExecuteAfterCommit(() => callbackOrder.Add(2));

        _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
        _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());

        // Act
        await _sut.CommitAsync(CancellationToken.None);

        // Assert
        Assert.False(_sut.IsInProgress);
        Assert.Equal(2, callbackOrder.Count);
        Assert.Equal(1, callbackOrder[0]);
        Assert.Equal(2, callbackOrder[1]);
        Assert.Equal(2, await CountPersistedOutboxEntries());
    }

    [Fact]
    public async Task RollbackAsync_WithoutActiveTransaction_DoesNotThrow()
    {
        // Act
        await _sut.RollbackAsync(CancellationToken.None);

        // Assert
        Assert.False(_sut.IsInProgress);
    }

    [Fact]
    public async Task RollbackAsync_WithActiveTransaction_DoesNotPersistChangesAndDoesNotExecuteAfterCommitActions()
    {
        // Arrange
        int callbackExecutionCount = 0;

        await _sut.BeginAsync(CancellationToken.None);

        _sut.ExecuteAfterCommit(() => callbackExecutionCount++);
        _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());

        // Act
        await _sut.RollbackAsync(CancellationToken.None);

        // Assert
        Assert.False(_sut.IsInProgress);
        Assert.Equal(0, callbackExecutionCount);
        Assert.Equal(0, await CountPersistedOutboxEntries());
    }

    [Fact]
    public async Task DisposeAsync_WithoutActiveTransaction_DoesNotThrow()
    {
        // Act & Assert
        await _sut.DisposeAsync();
    }

    [Fact]
    public async Task DisposeAsync_WithActiveTransaction_DoesNotThrow()
    {
        // Arrange
        await _sut.BeginAsync(CancellationToken.None);
        _sut.ExecuteAfterCommit(() => throw new InvalidOperationException("Callback should not run during dispose."));

        // Act & Assert
        await _sut.DisposeAsync();
    }

    [Fact]
    public void Dispose_WithoutActiveTransaction_DoesNotThrow()
    {
        // Act & Assert
        _sut.Dispose();
    }

    [Fact]
    public async Task Dispose_WithActiveTransaction_DoesNotThrow()
    {
        // Arrange
        await _sut.BeginAsync(CancellationToken.None);
        _sut.ExecuteAfterCommit(() => throw new InvalidOperationException("Callback should not run during dispose."));

        // Act & Assert
        _sut.Dispose();
    }

    private async Task<int> CountPersistedOutboxEntries()
    {
        await using var verificationDbContext = new TestDbContext(_dbContextOptions);
        return await verificationDbContext
            .Set<OutboxEntry>()
            .AsNoTracking()
            .CountAsync();
    }

    private static OutboxEntry CreateOutboxEntry()
    {
        DateTime now = DateTime.UtcNow;
        return new OutboxEntry
        {
            Id = Guid.NewGuid(),
            Event = "TestEvent",
            Handler = "TestHandler",
            Route = "END",
            Payload = "{}",
            Created = now,
            Updated = now
        };
    }

    public async ValueTask DisposeAsync()
    {
        await _sut.DisposeAsync();
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options);
}
