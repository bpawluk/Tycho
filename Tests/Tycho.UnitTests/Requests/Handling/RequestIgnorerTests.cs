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
        var cancellationToken = new CancellationToken();

        // Act
        await sut.HandleAsync(new TestRequest(), cancellationToken);

        // Assert
        // - no assertion required
    }

    [Fact]
    public async Task Handle_RequestWithResponse_ReturnsDefault()
    {
        // Arrange
        var sut = new RequestIgnorer<TestRequestWithResponse, string>();
        var cancellationToken = new CancellationToken();

        // Act
        var result = await sut.HandleAsync(new TestRequestWithResponse(), cancellationToken);

        // Assert
        Assert.Equal(default, result);
    }
}