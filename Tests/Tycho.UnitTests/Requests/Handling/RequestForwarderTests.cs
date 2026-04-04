using Moq;
using Tycho.Modules.Instance;
using Tycho.Requests.Handling;
using Tycho.UnitTests._Data.Modules;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Handling;

public class RequestForwarderTests
{
    [Fact]
    public async Task Handle_Request_CallsTargetModuleExecute()
    {
        // Arrange
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();

        var targetModuleMock = new Mock<IModule<TestModule>>();
        targetModuleMock.Setup(m => m.RequestBroker.ExecuteAsync(request, cancellationToken))
                        .Returns(Task.CompletedTask);

        var sut = new RequestForwarder<TestRequest, TestModule>(targetModuleMock.Object);

        // Act
        await sut.HandleAsync(request, cancellationToken);

        // Assert
        targetModuleMock.Verify(m => m.RequestBroker.ExecuteAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RequestWithResponse_CallsTargetModuleExecute()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var cancellationToken = new CancellationToken();
        var response = "success";

        var targetModuleMock = new Mock<IModule<TestModule>>();
        targetModuleMock.Setup(m => m.RequestBroker.ExecuteAsync<TestRequestWithResponse, string>(request, cancellationToken))
                        .ReturnsAsync(response);

        var sut = new RequestForwarder<TestRequestWithResponse, string, TestModule>(targetModuleMock.Object);

        // Act
        var result = await sut.HandleAsync(request, cancellationToken);

        // Assert
        Assert.Equal(response, result);
        targetModuleMock.Verify(
            m => m.RequestBroker.ExecuteAsync<TestRequestWithResponse, string>(
                request,
                cancellationToken),
            Times.Once);
    }
}