using Moq;
using Tycho.Requests.Handling;
using Tycho.Structure;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Handling;

public class MappedRequestExposerTests
{
    [Fact]
    public async Task Handle_Request_CallsParentExecute()
    {
        // Arrange
        var request = new TestRequest();
        var mappedRequest = new OtherRequest();
        var parentMock = new Mock<IParent>();

        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        mapMock.Setup(m => m(It.IsAny<TestRequest>()))
               .Returns(mappedRequest);

        var sut = new MappedRequestExposer<TestRequest, OtherRequest>(parentMock.Object, mapMock.Object);

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        mapMock.Verify(m => m(request), Times.Once);
        parentMock.Verify(p => p.Execute(mappedRequest, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_RequestWithResponse_CallsParentExecute()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var mappedRequest = new OtherRequestWithResponse();
        var response = "success";

        var parentMock = new Mock<IParent>();
        parentMock.Setup(p => p.Execute<OtherRequestWithResponse, string>(mappedRequest, CancellationToken.None))
                  .ReturnsAsync(response);

        var mapRequestMock = new Mock<Func<TestRequestWithResponse, OtherRequestWithResponse>>();
        mapRequestMock.Setup(m => m(It.IsAny<TestRequestWithResponse>()))
                      .Returns(mappedRequest);

        var mapResponseMock = new Mock<Func<string, string>>();
        mapResponseMock.Setup(m => m(It.IsAny<string>()))
                       .Returns((string response) => response);

        var sut = new MappedRequestExposer<
            TestRequestWithResponse, string,
            OtherRequestWithResponse, string>(
                parentMock.Object,
                mapRequestMock.Object,
                mapResponseMock.Object);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(response, result);
        mapRequestMock.Verify(m => m(request), Times.Once);
        parentMock.Verify(
            p => p.Execute<OtherRequestWithResponse, string>(mappedRequest, CancellationToken.None), 
            Times.Once);
        mapResponseMock.Verify(m => m(response), Times.Once);
    }
}