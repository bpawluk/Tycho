using Moq;
using Tycho.Requests.Handling;
using Tycho.Structure.Parent;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Handling;

public class RequestExposerTests
{
    [Fact]
    public async Task Handle_Request_CallsParentExecute()
    {
        // Arrange
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();

        var parentMock = new Mock<IParentReference>();
        parentMock.Setup(p => p.RequestBroker.ExecuteAsync(request, cancellationToken))
                  .Returns(Task.CompletedTask);

        var sut = new RequestExposer<TestRequest>(parentMock.Object);

        // Act
        await sut.HandleAsync(request, CancellationToken.None);

        // Assert
        parentMock.Verify(p => p.RequestBroker.ExecuteAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RequestWithResponse_CallsParentExecute()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var cancellationToken = new CancellationToken();
        string response = "success";

        var parentMock = new Mock<IParentReference>();
        parentMock.Setup(p => p.RequestBroker.ExecuteAsync<TestRequestWithResponse, string>(request, cancellationToken))
                  .ReturnsAsync(response);

        var sut = new RequestExposer<TestRequestWithResponse, string>(parentMock.Object);

        // Act
        string result = await sut.HandleAsync(request, cancellationToken);

        // Assert
        Assert.Equal(response, result);
        parentMock.Verify(p => p.RequestBroker.ExecuteAsync<TestRequestWithResponse, string>(request, cancellationToken), Times.Once);
    }
}
