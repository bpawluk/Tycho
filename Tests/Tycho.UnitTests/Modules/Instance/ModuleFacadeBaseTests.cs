using Moq;
using Tycho.Modules.Instance;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Modules.Instance;

public class ModuleFacadeBaseTests
{
    private readonly Mock<IModule> _moduleMock;
    private readonly Mock<IRequestBroker> _brokerMock;
    private readonly ConcreteModuleFacade _sut;

    public ModuleFacadeBaseTests()
    {
        _brokerMock = new Mock<IRequestBroker>();
        _moduleMock = new Mock<IModule>();
        _moduleMock.SetupGet(m => m.RequestBroker).Returns(_brokerMock.Object);
        _sut = new ConcreteModuleFacade(_moduleMock.Object);
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
        _brokerMock
            .Setup(b => b.ExecuteAsync<TestRequestWithResponse, string>(request, cancellationToken))
            .ReturnsAsync("result");

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
    public async Task StartAsync_DelegatesToUnderlyingModule()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _moduleMock.Setup(m => m.StartAsync(cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _sut.StartAsync(cancellationToken);

        // Assert
        _moduleMock.Verify(m => m.StartAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task StopAsync_DelegatesToUnderlyingModule()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _moduleMock.Setup(m => m.StopAsync(cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _sut.StopAsync(cancellationToken);

        // Assert
        _moduleMock.Verify(m => m.StopAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public void Dispose_DelegatesToUnderlyingModule()
    {
        // Act
        _sut.Dispose();

        // Assert
        _moduleMock.Verify(m => m.Dispose(), Times.Once);
    }

    private sealed class ConcreteModuleFacade(IModule module) : ModuleFacadeBase(module)
    {
        public Task Send<TRequest>(TRequest request, CancellationToken ct)
            where TRequest : class, IRequest
            => ExecuteAsync(request, ct);

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken ct)
            where TRequest : class, IRequest<TResponse>
            => ExecuteAsync<TRequest, TResponse>(request, ct);
    }
}
