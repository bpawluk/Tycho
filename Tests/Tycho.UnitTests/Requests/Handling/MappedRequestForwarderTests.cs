using Moq;
using Tycho.Modules.Instance;
using Tycho.Requests.Handling;
using Tycho.UnitTests._Data.Modules;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Handling;

public class MappedRequestForwarderTests
{
    [Fact]
    public async Task Handle_Request_CallsTargetModuleExecute()
    {
        // Arrange
        var request = new TestRequest();
        var mappedRequest = new OtherRequest();
        var cancellationToken = new CancellationToken();

        var targetModuleMock = new Mock<IModule<TestModule>>();
        targetModuleMock.Setup(m => m.RequestBroker.ExecuteAsync(mappedRequest, cancellationToken))
                        .Returns(Task.CompletedTask);

        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        mapMock.Setup(m => m(It.IsAny<TestRequest>()))
               .Returns(mappedRequest);

        var sut = new MappedRequestForwarder<TestRequest, OtherRequest, TestModule>(
            targetModuleMock.Object, mapMock.Object);

        // Act
        await sut.HandleAsync(request, cancellationToken);

        // Assert
        mapMock.Verify(m => m(request), Times.Once);
        targetModuleMock.Verify(m => m.RequestBroker.ExecuteAsync(mappedRequest, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RequestWithResponse_CallsTargetModuleExecute()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var mappedRequest = new OtherRequestWithResponse();
        var cancellationToken = new CancellationToken();
        string response = "success";

        var targetModuleMock = new Mock<IModule<TestModule>>();
        targetModuleMock.Setup(m => m.RequestBroker.ExecuteAsync<OtherRequestWithResponse, string>(mappedRequest, cancellationToken))
                        .ReturnsAsync(response);

        var mapRequestMock = new Mock<Func<TestRequestWithResponse, OtherRequestWithResponse>>();
        mapRequestMock.Setup(m => m(It.IsAny<TestRequestWithResponse>()))
                      .Returns(mappedRequest);

        var mapResponseMock = new Mock<Func<string, string>>();
        mapResponseMock.Setup(m => m(It.IsAny<string>()))
                       .Returns((string response) => response);

        var sut = new MappedRequestForwarder<
            TestRequestWithResponse, string,
            OtherRequestWithResponse, string,
            TestModule>(
                targetModuleMock.Object,
                mapRequestMock.Object,
                mapResponseMock.Object);

        // Act
        string result = await sut.HandleAsync(request, cancellationToken);

        // Assert
        Assert.Equal(response, result);
        mapRequestMock.Verify(m => m(request), Times.Once);
        targetModuleMock.Verify(
            m => m.RequestBroker.ExecuteAsync<OtherRequestWithResponse, string>(mappedRequest, cancellationToken),
            Times.Once);
        mapResponseMock.Verify(m => m(response), Times.Once);
    }
}
