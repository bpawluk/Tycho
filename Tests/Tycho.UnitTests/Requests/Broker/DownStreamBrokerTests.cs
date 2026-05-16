using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.Requests.Registrating.Registrations;
using Tycho.Structure;
using Tycho.Transactions;
using Tycho.UnitTests._Data.Modules;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Broker;

public class DownStreamBrokerTests
{
    private readonly Internals _internals;
    private readonly Mock<ITransaction> _transactionMock;

    private readonly DownStreamBroker<TestModule> _sut;

    public DownStreamBrokerTests()
    {
        _internals = new Internals(typeof(object));
        _sut = new DownStreamBroker<TestModule>(_internals);

        _transactionMock = new Mock<ITransaction>();
        _transactionMock.SetupGet(t => t.IsInProgress)
                        .Returns(false);
        _transactionMock.Setup(t => t.BeginAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);
        _transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);
        _transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        _internals.GetServiceCollection().AddSingleton(_transactionMock.Object);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestThatIsRegistered_ReturnsTrue(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequest, TestModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequest>();

        // Assert
        Assert.True(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestThatIsRegisteredUpstream_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IUpStreamRequestRegistration<TestRequest>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequest>();

        // Assert
        Assert.False(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestThatIsRegisteredForOtherModule_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequest, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequest>();

        // Assert
        Assert.False(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_MissingRequest_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequest>();

        // Assert
        Assert.False(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestWithResponseThatIsRegistered_ReturnsTrue(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequestWithResponse, string, TestModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequestWithResponse, string>();

        // Assert
        Assert.True(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestWithResponseThatIsRegisteredUpstream_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IUpStreamRequestRegistration<TestRequestWithResponse, string>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequestWithResponse, string>();

        // Assert
        Assert.False(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestWithResponseThatIsRegisteredForOtherModule_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequestWithResponse, string>();

        // Assert
        Assert.False(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_MissingRequestWithResponse_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequestWithResponse, string>();

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    public async Task Execute_RequestThatIsRegistered_CallsHandler()
    {
        // Arrange
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();

        var handlerMock = new Mock<IRequestHandler<TestRequest>>();

        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequest, TestModule>>();
        registrationMock.Setup(x => x.Handler).Returns(handlerMock.Object);

        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);
        _internals.Build();

        // Act
        await _sut.ExecuteAsync(request, cancellationToken);

        // Assert
        handlerMock.Verify(h => h.HandleAsync(request, cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Execute_RequestThatIsRegistered_WhenHandlerThrows_PropagatesExceptionWithoutTransaction()
    {
        // Arrange
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();

        var handlerMock = new Mock<IRequestHandler<TestRequest>>();
        handlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                   .ThrowsAsync(new InvalidOperationException("handler failure"));

        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequest, TestModule>>();
        registrationMock.Setup(x => x.Handler)
                        .Returns(handlerMock.Object);

        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);
        _internals.Build();

        // Act
        async Task Act()
        {
            await _sut.ExecuteAsync(request, cancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Execute_RequestThatIsRegistered_AndTransactionalHandler_BeginsAndCommitsTransaction()
    {
        // Arrange
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();

        _transactionMock.SetupGet(t => t.IsInProgress)
                        .Returns(true);

        var handlerMock = new Mock<ITransactionalRequestHandler<TestRequest>>();
        handlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                   .Returns(Task.CompletedTask);

        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequest, TestModule>>();
        registrationMock.Setup(x => x.Handler)
                        .Returns(handlerMock.Object);

        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);
        _internals.Build();

        // Act
        await _sut.ExecuteAsync(request, cancellationToken);

        // Assert
        handlerMock.Verify(h => h.HandleAsync(request, cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Execute_RequestThatIsRegistered_WhenTransactionalHandlerThrows_RollbacksAndPropagatesException()
    {
        // Arrange
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();

        _transactionMock.SetupGet(t => t.IsInProgress)
                        .Returns(true);

        var handlerMock = new Mock<ITransactionalRequestHandler<TestRequest>>();
        handlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                   .ThrowsAsync(new InvalidOperationException("handler failure"));

        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequest, TestModule>>();
        registrationMock.Setup(x => x.Handler)
                        .Returns(handlerMock.Object);

        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);
        _internals.Build();

        // Act
        async Task Act()
        {
            await _sut.ExecuteAsync(request, cancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Execute_MissingRequest_ThrowsInvalidOperationException()
    {
        // Arrange
        _internals.Build();
        var cancellationToken = new CancellationToken();

        // Act
        async Task Act()
        {
            await _sut.ExecuteAsync(new TestRequest(), cancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task Execute_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        TestRequest requestData = null!;
        var cancellationToken = new CancellationToken();

        // Act
        async Task Act()
        {
            await _sut.ExecuteAsync(requestData, cancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }

    [Fact]
    public async Task Execute_RequestWithResponseThatIsRegistered_CallsHandler()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        string response = "success";
        var cancellationToken = new CancellationToken();

        var handlerMock = new Mock<IRequestHandler<TestRequestWithResponse, string>>();
        handlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                   .ReturnsAsync(response);

        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequestWithResponse, string, TestModule>>();
        registrationMock.Setup(x => x.Handler)
                        .Returns(handlerMock.Object);

        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);
        _internals.Build();

        // Act
        string result = await _sut.ExecuteAsync<TestRequestWithResponse, string>(request, cancellationToken);

        // Assert
        Assert.Equal(response, result);
        handlerMock.Verify(h => h.HandleAsync(request, cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Execute_RequestWithResponseThatIsRegistered_WhenHandlerThrows_PropagatesExceptionWithoutTransaction()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var cancellationToken = new CancellationToken();

        var handlerMock = new Mock<IRequestHandler<TestRequestWithResponse, string>>();
        handlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                   .ThrowsAsync(new InvalidOperationException("handler failure"));

        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequestWithResponse, string, TestModule>>();
        registrationMock.Setup(x => x.Handler)
                        .Returns(handlerMock.Object);

        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);
        _internals.Build();

        // Act
        async Task Act()
        {
            await _sut.ExecuteAsync<TestRequestWithResponse, string>(request, cancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Execute_RequestWithResponseThatIsRegistered_AndTransactionalHandler_BeginsAndCommitsTransaction()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        string response = "success";
        var cancellationToken = new CancellationToken();

        _transactionMock.SetupGet(t => t.IsInProgress)
                        .Returns(true);

        var handlerMock = new Mock<ITransactionalRequestHandler<TestRequestWithResponse, string>>();
        handlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                   .ReturnsAsync(response);

        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequestWithResponse, string, TestModule>>();
        registrationMock.Setup(x => x.Handler)
                        .Returns(handlerMock.Object);

        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);
        _internals.Build();

        // Act
        string result = await _sut.ExecuteAsync<TestRequestWithResponse, string>(request, cancellationToken);

        // Assert
        Assert.Equal(response, result);
        handlerMock.Verify(h => h.HandleAsync(request, cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Execute_RequestWithResponseThatIsRegistered_WhenTransactionalHandlerThrows_RollbacksAndPropagatesException()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var cancellationToken = new CancellationToken();

        _transactionMock.SetupGet(t => t.IsInProgress)
                        .Returns(true);

        var handlerMock = new Mock<ITransactionalRequestHandler<TestRequestWithResponse, string>>();
        handlerMock.Setup(h => h.HandleAsync(request, cancellationToken))
                   .ThrowsAsync(new InvalidOperationException("handler failure"));

        var registrationMock = new Mock<IDownStreamRequestRegistration<TestRequestWithResponse, string, TestModule>>();
        registrationMock.Setup(x => x.Handler)
                        .Returns(handlerMock.Object);

        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);
        _internals.Build();

        // Act
        async Task Act()
        {
            await _sut.ExecuteAsync<TestRequestWithResponse, string>(request, cancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Execute_MissingRequestWithResponse_ThrowsInvalidOperationException()
    {
        // Arrange
        _internals.Build();
        var cancellationToken = new CancellationToken();

        // Act
        async Task Act()
        {
            await _sut.ExecuteAsync<TestRequestWithResponse, string>(new TestRequestWithResponse(), cancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task Execute_NullRequestWithResponse_ThrowsArgumentNullException()
    {
        // Arrange
        TestRequestWithResponse requestData = null!;
        var cancellationToken = new CancellationToken();

        // Act
        async Task Act()
        {
            await _sut.ExecuteAsync<TestRequestWithResponse, string>(requestData, cancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }
}
