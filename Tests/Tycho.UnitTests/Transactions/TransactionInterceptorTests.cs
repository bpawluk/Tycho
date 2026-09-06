using Moq;
using Tycho.Requests;
using Tycho.Transactions;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Transactions;

public class TransactionInterceptorTests
{
    [Fact]
    public async Task InterceptAsync_ExecutesHandlerInsideTransactionAndReturnsResponse()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var calls = new List<string>();
        var transactionMock = new Mock<ITransaction>();
        transactionMock
            .Setup(transaction => transaction.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task<string>>>(),
                cancellationToken))
            .Returns(async (Func<CancellationToken, Task<string>> operation, CancellationToken token) =>
            {
                calls.Add("transaction-before");
                string result = await operation(token);
                calls.Add("transaction-after");
                return result;
            });

        Task<string> Next(TestRequestWithResponse request, CancellationToken token)
        {
            Assert.Equal(cancellationToken, token);
            calls.Add("handler");
            return Task.FromResult("response");
        }

        var sut = new TransactionInterceptor<TestRequestWithResponse, string>(transactionMock.Object);

        string response = await sut.InterceptAsync(Next, new TestRequestWithResponse(), cancellationToken);

        Assert.Equal("response", response);
        Assert.Equal(["transaction-before", "handler", "transaction-after"], calls);
        transactionMock.Verify(transaction => transaction.ExecuteAsync(
            It.IsAny<Func<CancellationToken, Task<string>>>(),
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InterceptAsync_WhenTransactionFails_PropagatesFailure()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var handlerMock = new Mock<RequestHandlerDelegate<TestRequestWithResponse, string>>();
        var transactionMock = new Mock<ITransaction>();
        transactionMock
            .Setup(transaction => transaction.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task<string>>>(),
                cancellationToken))
            .ThrowsAsync(new InvalidOperationException("transaction failure"));

        var sut = new TransactionInterceptor<TestRequestWithResponse, string>(transactionMock.Object);

        Task Act() => sut.InterceptAsync(handlerMock.Object, new TestRequestWithResponse(), cancellationToken);

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Equal("transaction failure", exception.Message);
        handlerMock.Verify(handler => handler(
            It.IsAny<TestRequestWithResponse>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InterceptAsync_WhenHandlerFails_PropagatesFailureThroughTransaction()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var transactionMock = new Mock<ITransaction>();
        transactionMock
            .Setup(transaction => transaction.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task<string>>>(),
                cancellationToken))
            .Returns((Func<CancellationToken, Task<string>> operation, CancellationToken token) => operation(token));

        var sut = new TransactionInterceptor<TestRequestWithResponse, string>(transactionMock.Object);

        Task Act() => sut.InterceptAsync(
            (request, token) => throw new InvalidOperationException("handler failure"),
            new TestRequestWithResponse(),
            cancellationToken);

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Equal("handler failure", exception.Message);
    }

    [Fact]
    public async Task InterceptAsync_WithoutTransactionProvider_RejectsBeforeHandlerExecution()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var handlerMock = new Mock<RequestHandlerDelegate<TestRequestWithResponse, string>>();
        var sut = new TransactionInterceptor<TestRequestWithResponse, string>(new EmptyTransaction());

        Task Act() => sut.InterceptAsync(handlerMock.Object, new TestRequestWithResponse(), cancellationToken);

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(Act);
        Assert.Contains("No transaction provider is configured", exception.Message);
        handlerMock.Verify(handler => handler(
            It.IsAny<TestRequestWithResponse>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
