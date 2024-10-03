using Moq;
using Tycho.UnitTests._Data.Requests;
using TychoV2.Requests.Handling;
using TychoV2.Structure;

namespace Tycho.UnitTests.Requests.Handling;

public class RequestExposerTests
{
    [Fact]
    public async Task Handle_Request_CallsParentExecute()
    {
        // Arrange
        var request = new TestRequest();
        var parentMock = new Mock<IParent>();

        var sut = new RequestExposer<TestRequest>(parentMock.Object);

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        parentMock.Verify(p => p.Execute(request, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_RequestWithResponse_CallsParentExecute()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var response = "success";

        var parentMock = new Mock<IParent>();
        parentMock.Setup(p => p.Execute<TestRequestWithResponse, string>(request, CancellationToken.None))
                  .ReturnsAsync(response);

        var sut = new RequestExposer<TestRequestWithResponse, string>(parentMock.Object);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(response, result);
        parentMock.Verify(p => p.Execute<TestRequestWithResponse, string>(request, CancellationToken.None), Times.Once);
    }
}
