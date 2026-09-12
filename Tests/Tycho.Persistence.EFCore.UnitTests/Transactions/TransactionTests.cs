using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Transactions;

namespace Tycho.Persistence.EFCore.UnitTests.Transactions;

public sealed class TransactionTests : IAsyncLifetime
{
    private SqliteConnection _connection = default!;
    private DbContextOptions<TestDbContext> _dbOptions = default!;
    private TestDbContext _dbContext = default!;

    private Transaction _sut = default!;

    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        _dbOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new TestDbContext(_dbOptions);
        await _dbContext.Database.EnsureCreatedAsync();

        _sut = new Transaction(_dbContext);
    }

    [Fact]
    public async Task ExecuteAsync_CommitsChanges()
    {
        // Act
        await _sut.ExecuteAsync(async cancellationToken =>
        {
            Assert.NotNull(_dbContext.Database.CurrentTransaction);
            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
            await Task.Yield();
        }, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(_dbContext.Database.CurrentTransaction);
        Assert.Equal(1, await CountPersistedOutboxEntries());
    }

    [Fact]
    public async Task ExecuteAsyncForResult_CommitsChangesAndReturnsResult()
    {
        // Act
        string result = await _sut.ExecuteAsync(async cancellationToken =>
        {
            Assert.NotNull(_dbContext.Database.CurrentTransaction);
            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
            await Task.Yield();
            return "response";
        }, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("response", result);
        Assert.Null(_dbContext.Database.CurrentTransaction);
        Assert.Equal(1, await CountPersistedOutboxEntries());
    }

    [Fact]
    public async Task ExecuteAsync_WithNullOperation_Throws()
    {
        // Act
        Task Act() => _sut.ExecuteAsync(null!, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }

    [Fact]
    public async Task ExecuteAsyncForResult_WithNullOperation_Throws()
    {
        // Act
        Task Act() => _sut.ExecuteAsync<string>(null!, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOperationFails_RollsBackAndClearsTrackedState()
    {
        // Act
        Task Act() => _sut.ExecuteAsync(cancellationToken =>
        {
            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
            throw new InvalidOperationException("handler failure");
        }, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Null(_dbContext.Database.CurrentTransaction);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
        Assert.Equal(0, await CountPersistedOutboxEntries());
    }

    [Fact]
    public async Task ExecuteAsync_WhenOperationFailsAfterExplicitSaveChanges_RollsBackAndClearsTrackedState()
    {
        // Act
        Task Act() => _sut.ExecuteAsync(async cancellationToken =>
        {
            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
            await _dbContext.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("handler failure");
        }, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Null(_dbContext.Database.CurrentTransaction);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
        Assert.Equal(0, await CountPersistedOutboxEntries());
    }

    [Fact]
    public async Task ExecuteAsync_WhenAutomaticSaveChangesFails_RollsBackAndClearsTrackedState()
    {
        // Arrange
        OutboxEntry persistedEntry = CreateOutboxEntry();
        _dbContext.Set<OutboxEntry>().Add(persistedEntry);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        _dbContext.ChangeTracker.Clear();

        // Act
        Task Act() => _sut.ExecuteAsync(cancellationToken =>
        {
            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry(persistedEntry.Id));
            return Task.CompletedTask;
        }, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(Act);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
        Assert.Equal(1, await CountPersistedOutboxEntries());
    }

    [Fact]
    public async Task ExecuteAsync_WithConfiguredRetryStrategy_SuppressesRetriesInsideTransaction()
    {
        // Arrange
        DbContextOptions<TestDbContext> options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(
                _connection,
                sqlite => sqlite.ExecutionStrategy(dependencies => new TestRetryingExecutionStrategy(dependencies)))
            .Options;
        await using var dbContext = new TestDbContext(options);

        int operationCount = 0;
        var sut = new Transaction(dbContext);

        // Assert
        Assert.True(dbContext.Database.CreateExecutionStrategy().RetriesOnFailure);

        // Act
        async Task Act() => await sut.ExecuteAsync(cancellationToken =>
        {
            operationCount++;
            Assert.False(dbContext.Database.CreateExecutionStrategy().RetriesOnFailure);
            return Task.FromException<int>(new TestTransientException());
        }, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<TestTransientException>(Act);
        Assert.Equal(1, operationCount);
        Assert.True(dbContext.Database.CreateExecutionStrategy().RetriesOnFailure);
    }

    [Fact]
    public async Task ExecuteAsync_InsideRetryingStrategy_RejectsBeforeInvokingOperation()
    {
        // Arrange
        DbContextOptions<TestDbContext> options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(
                _connection,
                sqlite => sqlite.ExecutionStrategy(dependencies => new TestRetryingExecutionStrategy(dependencies)))
            .Options;
        await using var dbContext = new TestDbContext(options);

        IExecutionStrategy outerStrategy = dbContext.Database.CreateExecutionStrategy();

        int operationCount = 0;
        var sut = new Transaction(dbContext);

        // Act
        Task Act() => outerStrategy.ExecuteAsync(() => sut.ExecuteAsync(cancellationToken =>
        {
            operationCount++;
            return Task.CompletedTask;
        }, TestContext.Current.CancellationToken));

        // Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Equal("Transactions managed by Tycho cannot execute inside an active retrying EF Core execution strategy.", exception.Message);
        Assert.Equal(0, operationCount);
        Assert.Null(dbContext.Database.CurrentTransaction);
    }

    [Fact]
    public async Task ExecuteAsync_AfterOneExecution_RejectsReuse()
    {
        // Arrange
        await _sut.ExecuteAsync(cancellationToken => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Act
        Task Act() => _sut.ExecuteAsync(cancellationToken => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Equal("This transaction has already been executed or is currently in progress.", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancellationTokenIsCancelled_RollsBackUsingCleanupToken()
    {
        // Arrange
        using var cancellationSource = new CancellationTokenSource();

        // Act
        Task Act() => _sut.ExecuteAsync(cancellationToken =>
        {
            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
            cancellationSource.Cancel();
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }, cancellationSource.Token);

        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(Act);
        Assert.Null(_dbContext.Database.CurrentTransaction);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
        Assert.Equal(0, await CountPersistedOutboxEntries());
    }

    [Fact]
    public async Task ExecuteAsync_WhenDisposalFailsAfterCommit_ReturnsResult()
    {
        // Arrange
        var disposalFailure = new InvalidOperationException("disposal failure");
        Mock<IDbContextTransaction> transaction = InjectCleanupFailures(disposeFailure: disposalFailure);

        var logger = new Mock<ILogger<Transaction>>();
        var sut = new Transaction(_dbContext, logger.Object);

        // Act
        int result = await sut.ExecuteAsync(cancellationToken =>
        {
            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
            return Task.FromResult(42);
        }, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, result);
        Assert.Equal(1, await CountPersistedOutboxEntries());
        transaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        VerifyLoggedFailure(logger, disposalFailure);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommitFails_RollsBackAndClearsTrackedState()
    {
        // Arrange
        var commitFailure = new InvalidOperationException("commit failure");
        Mock<IDbContextTransaction> transaction = InjectCleanupFailures(commitFailure: commitFailure);

        var sut = new Transaction(_dbContext);

        // Act
        Task Act() => sut.ExecuteAsync(cancellationToken =>
        {
            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
            return Task.CompletedTask;
        }, TestContext.Current.CancellationToken);

        // Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Same(commitFailure, exception);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
        Assert.Equal(0, await CountPersistedOutboxEntries());
        transaction.Verify(t => t.RollbackAsync(CancellationToken.None), Times.Once);
        transaction.Verify(t => t.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRollbackAndDisposalFail_LogsCleanupFailuresAndRethrowsOriginal()
    {
        // Arrange
        var operationFailure = new OperationCanceledException("operation cancelled");
        var rollbackFailure = new InvalidOperationException("rollback failure");
        var disposalFailure = new InvalidOperationException("disposal failure");
        Mock<IDbContextTransaction> transaction = InjectCleanupFailures(rollbackFailure, disposalFailure);

        var logger = new Mock<ILogger<Transaction>>();
        var sut = new Transaction(_dbContext, logger.Object);
        using var cancellationSource = new CancellationTokenSource();

        // Act
        Task Act() => sut.ExecuteAsync(async cancellationToken =>
        {
            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
            await _dbContext.SaveChangesAsync(cancellationToken);
            cancellationSource.Cancel();
            throw operationFailure;
        }, cancellationSource.Token);

        OperationCanceledException exception = await Assert.ThrowsAsync<OperationCanceledException>(Act);

        // Assert
        Assert.Same(operationFailure, exception);
        VerifyLoggedFailure(logger, rollbackFailure);
        VerifyLoggedFailure(logger, disposalFailure);
        Assert.Empty(_dbContext.ChangeTracker.Entries());
        Assert.Equal(0, await CountPersistedOutboxEntries());
        Assert.Null(_dbContext.Database.CurrentTransaction);
        transaction.Verify(t => t.RollbackAsync(CancellationToken.None), Times.Once);
        transaction.Verify(t => t.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithOverlappingCalls_RejectsSecondOperation()
    {
        // Arrange
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var resume = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        // Act
        Task execution = _sut.ExecuteAsync(async cancellationToken =>
        {
            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
            entered.SetResult();
            await resume.Task.WaitAsync(cancellationToken);
        }, TestContext.Current.CancellationToken);

        await entered.Task.WaitAsync(TestContext.Current.CancellationToken);
        try
        {
            Assert.NotNull(_dbContext.Database.CurrentTransaction);
            int secondOperationCount = 0;
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ExecuteAsync(_ =>
            {
                secondOperationCount++;
                return Task.CompletedTask;
            }, TestContext.Current.CancellationToken));
            Assert.Equal(0, secondOperationCount);
        }
        finally
        {
            resume.SetResult();
            await execution;
        }

        // Assert
        Assert.Equal(1, await CountPersistedOutboxEntries());
        Assert.Null(_dbContext.Database.CurrentTransaction);
    }

    [Fact]
    public async Task ExecuteAsync_WhileExecuting_RejectsSecondOperation()
    {
        // Arrange
        int secondOperationCount = 0;

        // Act
        await _sut.ExecuteAsync(async cancellationToken =>
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ExecuteAsync(_ =>
            {
                secondOperationCount++;
                return Task.CompletedTask;
            }, cancellationToken));

            _dbContext.Set<OutboxEntry>().Add(CreateOutboxEntry());
        }, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, secondOperationCount);
        Assert.Equal(1, await CountPersistedOutboxEntries());
    }

    private Mock<IDbContextTransaction> InjectCleanupFailures(
        Exception? rollbackFailure = null,
        Exception? disposeFailure = null,
        Exception? commitFailure = null)
    {
        DatabaseFacade database = _dbContext.Database;
        var transactionMock = new Mock<IDbContextTransaction>();
        var databaseMock = new Mock<DatabaseFacade>(_dbContext) { CallBase = true };

        databaseMock
            .Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(async (CancellationToken cancellationToken) =>
            {
                IDbContextTransaction transaction = await database.BeginTransactionAsync(cancellationToken);

                transactionMock
                    .Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                    .Returns(async (CancellationToken token) =>
                    {
                        if (commitFailure is not null)
                        {
                            throw commitFailure;
                        }
                        await transaction.CommitAsync(token);
                    });

                transactionMock
                    .Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                    .Returns(async (CancellationToken token) =>
                    {
                        await transaction.RollbackAsync(token);
                        if (rollbackFailure is not null)
                        {
                            throw rollbackFailure;
                        }
                    });

                transactionMock
                    .Setup(t => t.DisposeAsync())
                    .Returns(async () =>
                    {
                        await transaction.DisposeAsync();
                        if (disposeFailure is not null)
                        {
                            throw disposeFailure;
                        }
                    });

                return transactionMock.Object;
            });

        _dbContext.DatabaseOverride = databaseMock.Object;
        return transactionMock;
    }

    private static void VerifyLoggedFailure(Mock<ILogger<Transaction>> logger, Exception exception)
    {
        logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            exception,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    private async Task<int> CountPersistedOutboxEntries()
    {
        await using var verificationDbContext = new TestDbContext(_dbOptions);
        return await verificationDbContext.Set<OutboxEntry>().AsNoTracking().CountAsync();
    }

    private static OutboxEntry CreateOutboxEntry(Guid? id = null)
    {
        DateTime now = DateTime.UtcNow;
        return new OutboxEntry
        {
            Id = id ?? Guid.NewGuid(),
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
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options)
    {
        public DatabaseFacade? DatabaseOverride { get; set; }

        public override DatabaseFacade Database => DatabaseOverride ?? base.Database;
    }

    private sealed class TestRetryingExecutionStrategy(ExecutionStrategyDependencies dependencies) : ExecutionStrategy(dependencies, 1, TimeSpan.Zero)
    {
        protected override bool ShouldRetryOn(Exception exception) => exception is TestTransientException;
    }

    private sealed class TestTransientException : Exception;
}
