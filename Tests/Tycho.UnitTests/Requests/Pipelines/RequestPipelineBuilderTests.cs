using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Requests;
using Tycho.Requests.Pipeline;
using Tycho.Transactions;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Pipelines;

public class RequestPipelineBuilderTests
{
    [Fact]
    public async Task Build_HandlerWithoutResponse_ExecutesHandlerAndReturnsNoResponse()
    {
        // Arrange
        var request = new TestRequest();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        var handlerMock = new Mock<IRequestHandler<TestRequest>>();
        handlerMock.Setup(x => x.HandleAsync(request, cancellationToken)).Returns(Task.CompletedTask);

        using ServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();
        RequestPipeline<TestRequest, NoResponse> sut = RequestPipelineBuilder.Build(serviceProvider, handlerMock.Object);

        // Act
        NoResponse response = await sut.ExecuteAsync(request, cancellationToken);

        // Assert
        Assert.Equal(NoResponse.Value, response);
        handlerMock.Verify(x => x.HandleAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Build_HandlerWithResponse_ReturnsHandlerResponse()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        var handlerMock = new Mock<IRequestHandler<TestRequestWithResponse, string>>();
        handlerMock.Setup(x => x.HandleAsync(request, cancellationToken)).ReturnsAsync("response");

        using ServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();
        RequestPipeline<TestRequestWithResponse, string> sut = RequestPipelineBuilder.Build(serviceProvider, handlerMock.Object);

        // Act
        string response = await sut.ExecuteAsync(request, cancellationToken);

        // Assert
        Assert.Equal("response", response);
        handlerMock.Verify(x => x.HandleAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Build_MultipleInterceptors_ExecutesInRegistrationOrderAroundHandler()
    {
        // Arrange
        var calls = new List<string>();
        var services = new ServiceCollection();

        services.AddSingleton(CreateInterceptor("first", calls).Object);
        services.AddSingleton(CreateInterceptor("second", calls).Object);

        var handlerMock = new Mock<IRequestHandler<TestRequestWithResponse, string>>();
        handlerMock.Setup(x => x.HandleAsync(It.IsAny<TestRequestWithResponse>(), It.IsAny<CancellationToken>()))
                   .Callback(() => calls.Add("handler"))
                   .ReturnsAsync("response");

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        RequestPipeline<TestRequestWithResponse, string> sut = RequestPipelineBuilder.Build(serviceProvider, handlerMock.Object);

        // Act
        string response = await sut.ExecuteAsync(new TestRequestWithResponse(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("response", response);
        Assert.Equal(["first-before", "second-before", "handler", "second-after", "first-after"], calls);
    }

    [Fact]
    public async Task Build_TransactionalHandlerWithoutResponse_WrapsHandlerInTransaction()
    {
        // Arrange
        var calls = new List<string>();
        Mock<ITransaction> transactionMock = CreateTransaction(calls);

        var handlerMock = new Mock<ITransactionalRequestHandler<TestRequest>>();
        handlerMock.Setup(x => x.HandleAsync(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
                   .Callback(() => calls.Add("handler"))
                   .Returns(Task.CompletedTask);

        using ServiceProvider serviceProvider = new ServiceCollection().AddSingleton(transactionMock.Object).BuildServiceProvider();
        RequestPipeline<TestRequest, NoResponse> sut = RequestPipelineBuilder.Build(serviceProvider, handlerMock.Object);

        // Act
        await sut.ExecuteAsync(new TestRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(["begin", "handler", "commit"], calls);
    }

    [Fact]
    public async Task Build_TransactionalHandlerWithResponse_PlacesRegisteredInterceptorsOutsideTransaction()
    {
        // Arrange
        var calls = new List<string>();
        Mock<ITransaction> transactionMock = CreateTransaction(calls);

        var services = new ServiceCollection();
        services.AddSingleton(transactionMock.Object);
        services.AddSingleton(CreateInterceptor("interceptor", calls).Object);

        var handlerMock = new Mock<ITransactionalRequestHandler<TestRequestWithResponse, string>>();
        handlerMock.Setup(x => x.HandleAsync(It.IsAny<TestRequestWithResponse>(), It.IsAny<CancellationToken>()))
                   .Callback(() => calls.Add("handler"))
                   .ReturnsAsync("response");

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        RequestPipeline<TestRequestWithResponse, string> sut = RequestPipelineBuilder.Build(serviceProvider, handlerMock.Object);

        // Act
        string response = await sut.ExecuteAsync(new TestRequestWithResponse(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("response", response);
        Assert.Equal(["interceptor-before", "begin", "handler", "commit", "interceptor-after"], calls);
    }

    private static Mock<IRequestInterceptor<TestRequestWithResponse, string>> CreateInterceptor(string name, List<string> calls)
    {
        var interceptorMock = new Mock<IRequestInterceptor<TestRequestWithResponse, string>>();
        interceptorMock.Setup(x => x.InterceptAsync(
                           It.IsAny<RequestHandlerDelegate<TestRequestWithResponse, string>>(),
                           It.IsAny<TestRequestWithResponse>(),
                           It.IsAny<CancellationToken>()))
                       .Returns(async (RequestHandlerDelegate<TestRequestWithResponse, string> next, TestRequestWithResponse request, CancellationToken token) =>
                       {
                           calls.Add($"{name}-before");
                           string response = await next(request, token);
                           calls.Add($"{name}-after");
                           return response;
                       });
        return interceptorMock;
    }

    private static Mock<ITransaction> CreateTransaction(List<string> calls)
    {
        var transactionMock = new Mock<ITransaction>();
        transactionMock.SetupGet(x => x.IsInProgress).Returns(true);
        transactionMock.Setup(x => x.BeginAsync(It.IsAny<CancellationToken>()))
                       .Callback(() => calls.Add("begin"))
                       .Returns(Task.CompletedTask);
        transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                       .Callback(() => calls.Add("commit"))
                       .Returns(Task.CompletedTask);
        return transactionMock;
    }
}
