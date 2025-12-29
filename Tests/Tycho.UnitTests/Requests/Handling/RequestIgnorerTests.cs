using Tycho.Requests.Handling;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Handling;

public class RequestIgnorerTests
{
    [Fact]
    public async Task Handle_Request_DoesNothing()
    {
        // Arrange
        var sut = new RequestIgnorer<TestRequest>();

        // Act
        await sut.Handle(new TestRequest(), CancellationToken.None);

        // Assert
        // - no assertion required
    }

    [Fact]
    public async Task Handle_RequestWithResponse_ReturnsDefault()
    {
        // Arrange
        var sut = new RequestIgnorer<TestRequestWithResponse, string>();

        // Act
        var result = await sut.Handle(new TestRequestWithResponse(), CancellationToken.None);

        // Assert
        Assert.Equal(default, result);
    }
}