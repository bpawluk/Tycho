using Moq;
using Tycho.Requests;
using Tycho.Requests.Pipeline;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Pipelines;

public class RequestPipelineTests
{
    [Fact]
    public async Task ExecuteAsync_NoInterceptors_ExecutesFinalStepAndReturnsItsResponse()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        var finalStepMock = new Mock<RequestHandlerDelegate<TestRequestWithResponse, string>>();
        finalStepMock.Setup(x => x(request, cancellationToken)).ReturnsAsync("response");

        var sut = new RequestPipeline<TestRequestWithResponse, string>(finalStepMock.Object);

        // Act
        string response = await sut.ExecuteAsync(request, cancellationToken);

        // Assert
        Assert.Equal("response", response);
        finalStepMock.Verify(x => x(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_InterceptorsAdded_ExecutesThemInReverseAdditionOrderAroundFinalStep()
    {
        // Arrange
        var calls = new List<string>();
        var request = new TestRequestWithResponse();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        Task<string> FinalStep(TestRequestWithResponse data, CancellationToken token)
        {
            Assert.Same(request, data);
            Assert.Equal(cancellationToken, token);
            calls.Add("handler");
            return Task.FromResult("response");
        }

        Mock<IRequestInterceptor<TestRequestWithResponse, string>> first = CreatePassingInterceptor("first", calls, request, cancellationToken);
        Mock<IRequestInterceptor<TestRequestWithResponse, string>> second = CreatePassingInterceptor("second", calls, request, cancellationToken);

        var sut = new RequestPipeline<TestRequestWithResponse, string>(FinalStep);
        sut.AddInterceptor(first.Object);
        sut.AddInterceptor(second.Object);

        // Act
        string response = await sut.ExecuteAsync(request, cancellationToken);

        // Assert
        Assert.Equal("response", response);
        Assert.Equal(["second-before", "first-before", "handler", "first-after", "second-after"], calls);
    }

    [Fact]
    public async Task ExecuteAsync_InterceptorShortCircuits_DoesNotExecuteFinalStep()
    {
        // Arrange
        var finalStepMock = new Mock<RequestHandlerDelegate<TestRequestWithResponse, string>>();
        var interceptorMock = new Mock<IRequestInterceptor<TestRequestWithResponse, string>>();
        interceptorMock.Setup(x => x.InterceptAsync(
                           It.IsAny<RequestHandlerDelegate<TestRequestWithResponse, string>>(),
                           It.IsAny<TestRequestWithResponse>(),
                           It.IsAny<CancellationToken>()))
                       .ReturnsAsync("short-circuited");

        var sut = new RequestPipeline<TestRequestWithResponse, string>(finalStepMock.Object);
        sut.AddInterceptor(interceptorMock.Object);

        // Act
        string response = await sut.ExecuteAsync(new TestRequestWithResponse(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("short-circuited", response);
        finalStepMock.Verify(x => x(It.IsAny<TestRequestWithResponse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IRequestInterceptor<TestRequestWithResponse, string>> CreatePassingInterceptor(
        string name,
        List<string> calls,
        TestRequestWithResponse expectedRequest,
        CancellationToken expectedToken)
    {
        var interceptorMock = new Mock<IRequestInterceptor<TestRequestWithResponse, string>>();
        interceptorMock.Setup(x => x.InterceptAsync(
                           It.IsAny<RequestHandlerDelegate<TestRequestWithResponse, string>>(),
                           expectedRequest,
                           expectedToken))
                       .Returns(async (RequestHandlerDelegate<TestRequestWithResponse, string> next, TestRequestWithResponse request, CancellationToken token) =>
                       {
                           calls.Add($"{name}-before");
                           string response = await next(request, token);
                           calls.Add($"{name}-after");
                           return response;
                       });
        return interceptorMock;
    }
}
