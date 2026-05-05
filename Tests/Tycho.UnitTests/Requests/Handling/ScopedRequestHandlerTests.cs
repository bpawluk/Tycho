using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Requests;
using Tycho.Requests.Handling;
using Tycho.Structure;
using Tycho.Transactions;
using Tycho.UnitTests._Data.Modules;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Handling;

public class ScopedRequestHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithRegularHandler_HandlesTheRequest()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        var handlerMock = new Mock<IRequestHandler<TestRequest>>();
        var internals = CreateInternals(handlerMock.Object);
        var sut = new ScopedRequestHandler<TestRequest, IRequestHandler<TestRequest>>(internals);
        var request = new TestRequest();

        // Act
        await sut.HandleAsync(request, cancellationToken);

        // Assert
        handlerMock.Verify(h => h.HandleAsync(request, cancellationToken));
    }

    [Fact]
    public async Task HandleAsync_WithRegularHandler_AndTheHandlerThrowingException_RethrowsTheException()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new TestRequest();

        var handlerMock = new Mock<IRequestHandler<TestRequest>>();
        handlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                   .ThrowsAsync(new InvalidOperationException());

        var internals = CreateInternals(handlerMock.Object);
        var sut = new ScopedRequestHandler<TestRequest, IRequestHandler<TestRequest>>(internals);

        // Act
        Task Act() => sut.HandleAsync(request, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task HandleAsync_WithTransactionalHandler_HandlesTheRequestWithinTransaction()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new TestRequest();

        var sequence = 0;
        var transactionalHandlerMock = new Mock<ITransactionalRequestHandler<TestRequest>>();

        transactionalHandlerMock.When(() => sequence == 0)
                                .Setup(h => h.BeginTransactionAsync(cancellationToken))
                                .Callback(() => sequence++)
                                .Returns(Task.CompletedTask);

        transactionalHandlerMock.When(() => sequence != 0)
                                .Setup(h => h.BeginTransactionAsync(cancellationToken))
                                .Throws(new InvalidOperationException());

        transactionalHandlerMock.When(() => sequence == 1)
                                .Setup(h => h.HandleAsync(request, cancellationToken))
                                .Callback(() => sequence++)
                                .Returns(Task.CompletedTask);

        transactionalHandlerMock.When(() => sequence != 1)
                                .Setup(h => h.HandleAsync(request, cancellationToken))
                                .Throws(new InvalidOperationException());

        transactionalHandlerMock.When(() => sequence == 2)
                                .Setup(h => h.CommitTransactionAsync(cancellationToken))
                                .Callback(() => sequence++)
                                .Returns(Task.CompletedTask);

        transactionalHandlerMock.When(() => sequence != 2)
                                .Setup(h => h.CommitTransactionAsync(cancellationToken))
                                .Throws(new InvalidOperationException());

        var internals = CreateInternals(transactionalHandlerMock.Object);
        var sut = new ScopedRequestHandler<TestRequest, ITransactionalRequestHandler<TestRequest>>(internals);

        // Act
        await sut.HandleAsync(request, cancellationToken);

        // Assert
        transactionalHandlerMock.Verify(h => h.BeginTransactionAsync(cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(h => h.HandleAsync(request, cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(h => h.CommitTransactionAsync(cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(h => h.RollbackTransactionAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithTransactionalHandler_AndTheHandlerThrowingException_RollsBackTheTransaction()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new TestRequest();

        var transactionalHandlerMock = new Mock<ITransactionalRequestHandler<TestRequest>>();
        transactionalHandlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                                .ThrowsAsync(new InvalidOperationException());

        var internals = CreateInternals(transactionalHandlerMock.Object);
        var sut = new ScopedRequestHandler<TestRequest, ITransactionalRequestHandler<TestRequest>>(internals);

        // Act
        Task Act() => sut.HandleAsync(request, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        transactionalHandlerMock.Verify(h => h.BeginTransactionAsync(cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(h => h.HandleAsync(request, cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(h => h.CommitTransactionAsync(cancellationToken), Times.Never);
        transactionalHandlerMock.Verify(h => h.RollbackTransactionAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithResponseAndRegularHandler_HandlesTheRequestAndReturnsResponse()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new TestRequestWithResponse();
        const string response = "result";

        var handlerMock = new Mock<IRequestHandler<TestRequestWithResponse, string>>();
        handlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                   .ReturnsAsync(response);

        var internals = CreateInternals(handlerMock.Object);
        var sut = new ScopedRequestHandler<TestRequestWithResponse, string, IRequestHandler<TestRequestWithResponse, string>>(internals);

        // Act
        var result = await sut.HandleAsync(request, cancellationToken);

        // Assert
        Assert.Equal(response, result);
        handlerMock.Verify(h => h.HandleAsync(request, cancellationToken));
    }

    [Fact]
    public async Task HandleAsync_WithResponseAndRegularHandler_AndTheHandlerThrowingException_RethrowsTheException()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new TestRequestWithResponse();

        var handlerMock = new Mock<IRequestHandler<TestRequestWithResponse, string>>();
        handlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                   .ThrowsAsync(new InvalidOperationException());

        var internals = CreateInternals(handlerMock.Object);
        var sut = new ScopedRequestHandler<TestRequestWithResponse, string, IRequestHandler<TestRequestWithResponse, string>>(internals);

        // Act
        Task Act() => sut.HandleAsync(request, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task HandleAsync_WithResponseAndTransactionalHandler_HandlesTheRequestWithinTransaction()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new TestRequestWithResponse();
        const string response = "result";

        var sequence = 0;
        var transactionalHandlerMock = new Mock<ITransactionalRequestHandler<TestRequestWithResponse, string>>();

        transactionalHandlerMock.When(() => sequence == 0)
                                .Setup(h => h.BeginTransactionAsync(cancellationToken))
                                .Callback(() => sequence++)
                                .Returns(Task.CompletedTask);

        transactionalHandlerMock.When(() => sequence != 0)
                                .Setup(h => h.BeginTransactionAsync(cancellationToken))
                                .Throws(new InvalidOperationException());

        transactionalHandlerMock.When(() => sequence == 1)
                                .Setup(h => h.HandleAsync(request, cancellationToken))
                                .Callback(() => sequence++)
                                .ReturnsAsync(response);

        transactionalHandlerMock.When(() => sequence != 1)
                                .Setup(h => h.HandleAsync(request, cancellationToken))
                                .Throws(new InvalidOperationException());

        transactionalHandlerMock.When(() => sequence == 2)
                                .Setup(h => h.CommitTransactionAsync(cancellationToken))
                                .Callback(() => sequence++)
                                .Returns(Task.CompletedTask);

        transactionalHandlerMock.When(() => sequence != 2)
                                .Setup(h => h.CommitTransactionAsync(cancellationToken))
                                .Throws(new InvalidOperationException());

        var internals = CreateInternals(transactionalHandlerMock.Object);
        var sut = new ScopedRequestHandler<TestRequestWithResponse, string, ITransactionalRequestHandler<TestRequestWithResponse, string>>(internals);

        // Act
        var result = await sut.HandleAsync(request, cancellationToken);

        // Assert
        Assert.Equal(response, result);
        transactionalHandlerMock.Verify(h => h.BeginTransactionAsync(cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(h => h.HandleAsync(request, cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(h => h.CommitTransactionAsync(cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(h => h.RollbackTransactionAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithResponseAndTransactionalHandler_AndTheHandlerThrowingException_RollsBackTheTransaction()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new TestRequestWithResponse();

        var transactionalHandlerMock = new Mock<ITransactionalRequestHandler<TestRequestWithResponse, string>>();
        transactionalHandlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                                .ThrowsAsync(new InvalidOperationException());

        var internals = CreateInternals(transactionalHandlerMock.Object);
        var sut = new ScopedRequestHandler<TestRequestWithResponse, string, ITransactionalRequestHandler<TestRequestWithResponse, string>>(internals);

        // Act
        Task Act() => sut.HandleAsync(request, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        transactionalHandlerMock.Verify(h => h.BeginTransactionAsync(cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(h => h.HandleAsync(request, cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(h => h.CommitTransactionAsync(cancellationToken), Times.Never);
        transactionalHandlerMock.Verify(h => h.RollbackTransactionAsync(cancellationToken), Times.Once);
    }

    private static Internals CreateInternals<THandler>(THandler handler)
        where THandler : class
    {
        var internals = new Internals(typeof(TestModule));
        internals.GetServiceCollection().AddSingleton(handler);
        internals.Build();
        return internals;
    }
}
