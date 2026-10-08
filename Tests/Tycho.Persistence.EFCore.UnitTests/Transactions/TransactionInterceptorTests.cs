using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Persistence.EFCore.Transactions;

namespace Tycho.Persistence.EFCore.UnitTests.Transactions;

public sealed class TransactionInterceptorTests
{
    private readonly TransactionInterceptor _sut = new();
    private readonly DbTransaction _transaction = Mock.Of<DbTransaction>();
    private readonly EventDefinition _eventDefinition = new(
        Mock.Of<ILoggingOptions>(options => options.WarningsConfiguration == new WarningsConfiguration()),
        new EventId(1), LogLevel.Debug, "TestTransaction",
        _ => (_, _) => { });

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task TransactionCleanup_AfterRollbackOrFailure_RemovesOnlyAffectedCallback(bool failed, bool asynchronous)
    {
        // Arrange
        Guid affectedId = Guid.NewGuid();
        Guid unaffectedId = Guid.NewGuid();
        int affectedCalls = 0;
        int unaffectedCalls = 0;
        _sut.ExecuteAfterCommit(affectedId, () => affectedCalls++);
        _sut.ExecuteAfterCommit(unaffectedId, () => unaffectedCalls++);

        // Act
        await NotifyCleanup(affectedId, failed, asynchronous);
        await NotifyCleanup(affectedId, failed, asynchronous);
        await NotifyCleanup(Guid.NewGuid(), failed, asynchronous);
        await NotifyCommit(affectedId, asynchronous);
        await NotifyCommit(unaffectedId, asynchronous);
        await NotifyCommit(unaffectedId, asynchronous);
        await NotifyCommit(Guid.NewGuid(), asynchronous);

        // Assert
        Assert.Equal(0, affectedCalls);
        Assert.Equal(1, unaffectedCalls);
    }

    private Task NotifyCleanup(Guid transactionId, bool failed, bool asynchronous)
    {
        if (failed)
        {
            var eventData = new TransactionErrorEventData(
                _eventDefinition, (_, _) => "transaction failed", _transaction, null,
                transactionId, Guid.NewGuid(), asynchronous, "Commit",
                new InvalidOperationException("transaction failure"), DateTimeOffset.UtcNow, TimeSpan.Zero);

            if (asynchronous)
            {
                return _sut.TransactionFailedAsync(_transaction, eventData, TestContext.Current.CancellationToken);
            }

            _sut.TransactionFailed(_transaction, eventData);
        }
        else
        {
            TransactionEndEventData eventData = CreateEndEventData(transactionId, asynchronous);
            if (asynchronous)
            {
                return _sut.TransactionRolledBackAsync(_transaction, eventData, TestContext.Current.CancellationToken);
            }

            _sut.TransactionRolledBack(_transaction, eventData);
        }

        return Task.CompletedTask;
    }

    private Task NotifyCommit(Guid transactionId, bool asynchronous)
    {
        TransactionEndEventData eventData = CreateEndEventData(transactionId, asynchronous);
        if (asynchronous)
        {
            return _sut.TransactionCommittedAsync(_transaction, eventData, TestContext.Current.CancellationToken);
        }

        _sut.TransactionCommitted(_transaction, eventData);
        return Task.CompletedTask;
    }

    private TransactionEndEventData CreateEndEventData(Guid transactionId, bool asynchronous)
    {
        return new TransactionEndEventData(
            _eventDefinition, (_, _) => "transaction ended", _transaction, null,
            transactionId, Guid.NewGuid(), asynchronous, DateTimeOffset.UtcNow, TimeSpan.Zero);
    }
}
