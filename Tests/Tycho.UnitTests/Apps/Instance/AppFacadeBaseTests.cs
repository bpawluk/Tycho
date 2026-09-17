using Moq;
using Tycho.Apps.Instance;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Apps.Instance;

public class AppFacadeBaseTests
{
    private readonly Mock<IApp> _appMock;
    private readonly Mock<IRequestBroker> _brokerMock;
    private readonly ConcreteAppFacade _sut;

    public AppFacadeBaseTests()
    {
        _brokerMock = new Mock<IRequestBroker>();
        _appMock = new Mock<IApp>();
        _appMock.SetupGet(a => a.RequestBroker).Returns(_brokerMock.Object);
        _sut = new ConcreteAppFacade(_appMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_DelegatesToBroker()
    {
        // Arrange
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();
        _brokerMock.Setup(b => b.ExecuteAsync(request, cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _sut.Send(request, cancellationToken);

        // Assert
        _brokerMock.Verify(b => b.ExecuteAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequestWithResponse_DelegatesToBroker()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var cancellationToken = new CancellationToken();
        _brokerMock.Setup(b => b.ExecuteAsync<TestRequestWithResponse, string>(request, cancellationToken)).ReturnsAsync("result");

        // Act
        string result = await _sut.Send<TestRequestWithResponse, string>(request, cancellationToken);

        // Assert
        Assert.Equal("result", result);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act
        async Task Act() => await _sut.Send<TestRequest>(null!, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRequestWithResponse_ThrowsArgumentNullException()
    {
        // Act
        async Task Act() => await _sut.Send<TestRequestWithResponse, string>(null!, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }

    [Fact]
    public async Task StartAsync_DelegatesToUnderlyingApp()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _appMock.Setup(a => a.StartAsync(cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _sut.StartAsync(cancellationToken);

        // Assert
        _appMock.Verify(a => a.StartAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task StopAsync_DelegatesToUnderlyingApp()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _appMock.Setup(a => a.StopAsync(cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _sut.StopAsync(cancellationToken);

        // Assert
        _appMock.Verify(a => a.StopAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public void Dispose_DelegatesToUnderlyingApp()
    {
        // Act
        _sut.Dispose();

        // Assert
        _appMock.Verify(a => a.Dispose(), Times.Once);
    }

    private sealed class ConcreteAppFacade(IApp app) : AppFacadeBase(app)
    {
        public Task Send<TRequest>(TRequest request, CancellationToken ct)
            where TRequest : class, IRequest => ExecuteAsync(request, ct);

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken ct)
            where TRequest : class, IRequest<TResponse> => ExecuteAsync<TRequest, TResponse>(request, ct);
    }
}
