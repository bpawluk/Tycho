using Moq;
using Tycho.Requests;
using Tycho.Transactions;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Transactions;

public class TransactionInterceptorTests
{
    [Fact]
    public async Task InterceptAsync_HandlerCompletes_BeginsAndCommitsTransaction()
    {
        // Arrange
        var calls = new List<string>();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        Task<string> Next(TestRequestWithResponse request, CancellationToken token)
        {
            calls.Add("handler");
            Assert.Equal(cancellationToken, token);
            return Task.FromResult("response");
        }
        Mock<ITransaction> transactionMock = CreateTransaction(calls, cancellationToken);

        var sut = new TransactionInterceptor<TestRequestWithResponse, string>(transactionMock.Object);

        // Act
        string response = await sut.InterceptAsync(Next, new TestRequestWithResponse(), cancellationToken);

        // Assert
        Assert.Equal("response", response);
        Assert.Equal(["begin", "handler", "commit"], calls);
        transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InterceptAsync_HandlerThrows_RollsBackAndPropagates()
    {
        // Arrange
        var calls = new List<string>();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        Task<string> Next(TestRequestWithResponse request, CancellationToken token)
        {
            calls.Add("handler");
            throw new InvalidOperationException("handler failure");
        }
        Mock<ITransaction> transactionMock = CreateTransaction(calls, cancellationToken);

        var sut = new TransactionInterceptor<TestRequestWithResponse, string>(transactionMock.Object);

        // Act
        Task Act() => sut.InterceptAsync(Next, new TestRequestWithResponse(), cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Equal(["begin", "handler", "rollback"], calls);
        transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InterceptAsync_CommitThrows_RollsBackAndPropagates()
    {
        // Arrange
        var calls = new List<string>();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        Task<string> Next(TestRequestWithResponse request, CancellationToken token)
        {
            calls.Add("handler");
            Assert.Equal(cancellationToken, token);
            return Task.FromResult("response");
        }

        Mock<ITransaction> transactionMock = CreateTransaction(calls, cancellationToken);
        transactionMock.Setup(x => x.CommitAsync(cancellationToken))
                       .Callback(() => calls.Add("commit"))
                       .ThrowsAsync(new InvalidOperationException("commit failure"));

        var sut = new TransactionInterceptor<TestRequestWithResponse, string>(transactionMock.Object);

        // Act
        Task Act() => sut.InterceptAsync(Next, new TestRequestWithResponse(), cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Equal(["begin", "handler", "commit", "rollback"], calls);
    }

    [Fact]
    public async Task InterceptAsync_TransactionIsInactive_DoesNotCommitOrRollback()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        var transactionMock = new Mock<ITransaction>();
        transactionMock.SetupGet(x => x.IsInProgress)
                       .Returns(false);
        transactionMock.Setup(x => x.BeginAsync(cancellationToken))
                       .Returns(Task.CompletedTask);

        var sut = new TransactionInterceptor<TestRequestWithResponse, string>(transactionMock.Object);

        // Act
        string response = await sut.InterceptAsync(
            (request, token) => Task.FromResult("response"),
            new TestRequestWithResponse(),
            cancellationToken);

        // Assert
        Assert.Equal("response", response);
        transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InterceptAsync_BeginThrows_DoesNotCallCommitOrHandlerOrRollback()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        var handlerMock = new Mock<RequestHandlerDelegate<TestRequestWithResponse, string>>();

        var transactionMock = new Mock<ITransaction>();
        transactionMock.Setup(x => x.BeginAsync(cancellationToken))
                       .ThrowsAsync(new InvalidOperationException("begin failure"));

        var sut = new TransactionInterceptor<TestRequestWithResponse, string>(transactionMock.Object);

        // Act
        Task Act() => sut.InterceptAsync(handlerMock.Object, new TestRequestWithResponse(), cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        handlerMock.Verify(x => x(It.IsAny<TestRequestWithResponse>(), It.IsAny<CancellationToken>()), Times.Never);
        transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<ITransaction> CreateTransaction(List<string> calls, CancellationToken cancellationToken)
    {
        var transactionMock = new Mock<ITransaction>();
        transactionMock.SetupGet(x => x.IsInProgress).Returns(true);
        transactionMock.Setup(x => x.BeginAsync(cancellationToken))
                       .Callback(() => calls.Add("begin"))
                       .Returns(Task.CompletedTask);
        transactionMock.Setup(x => x.CommitAsync(cancellationToken))
                       .Callback(() => calls.Add("commit"))
                       .Returns(Task.CompletedTask);
        transactionMock.Setup(x => x.RollbackAsync(cancellationToken))
                       .Callback(() => calls.Add("rollback"))
                       .Returns(Task.CompletedTask);
        return transactionMock;
    }
}
