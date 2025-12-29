using Moq;
using Tycho.Requests.Handling;
using Tycho.Structure;
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
        var targetModuleMock = new Mock<IModule<TestModule>>();

        var sut = new RequestForwarder<TestRequest, TestModule>(targetModuleMock.Object);

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        targetModuleMock.Verify(m => m.Execute(request, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_RequestWithResponse_CallsTargetModuleExecute()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var response = "success";

        var targetModuleMock = new Mock<IModule<TestModule>>();
        targetModuleMock.Setup(m => m.Execute<TestRequestWithResponse, string>(request, CancellationToken.None))
                        .ReturnsAsync(response);

        var sut = new RequestForwarder<TestRequestWithResponse, string, TestModule>(targetModuleMock.Object);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(response, result);
        targetModuleMock.Verify(
            m => m.Execute<TestRequestWithResponse, string>(
                request, 
                CancellationToken.None),
            Times.Once);
    }
}