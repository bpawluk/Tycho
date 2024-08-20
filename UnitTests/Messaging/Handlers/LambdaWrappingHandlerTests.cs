using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using UnitTests.Utils;

namespace UnitTests.Messaging.Handlers;

public class LambdaWrappingHandlerTests
{
    [Fact]
    public async Task LambdaWrappingEventHandler_WrapsAction()
    {
        // Arrange
        TestEvent? passedEvent = null;
        var lambda = (TestEvent eventData) => { passedEvent = eventData; };
        var handler = new LambdaWrappingEventHandler<TestEvent>(lambda);

        // Act
        var eventToPublish = new TestEvent("test-event");
        await handler.Handle(eventToPublish, CancellationToken.None);

        // Assert
        Assert.Equal(eventToPublish, passedEvent);
    }

    [Fact]
    public async Task LambdaWrappingEventHandler_WrapsAsyncAction()
    {
        // Arrange
        TestEvent? passedEvent = null;
        var lambda = async (TestEvent eventData) => 
        { 
            await Task.Delay(100);
            passedEvent = eventData; 
        };
        var handler = new LambdaWrappingEventHandler<TestEvent>(lambda);

        // Act
        var eventToPublish = new TestEvent("test-event");
        await handler.Handle(eventToPublish, CancellationToken.None);

        // Assert
        Assert.Equal(eventToPublish, passedEvent);
    }

    [Fact]
    public async Task LambdaWrappingEventHandler_WrapsAsyncActionWithToken()
    {
        // Arrange
        TestEvent? passedEvent = null;
        CancellationToken? passedToken = null;
        var lambda = async (TestEvent eventData, CancellationToken token) =>
        {
            await Task.Delay(100, token);
            passedEvent = eventData;
            passedToken = token;
        };
        var handler = new LambdaWrappingEventHandler<TestEvent>(lambda);

        // Act
        var eventToPublish = new TestEvent("test-event");
        var tokenToPass = new CancellationToken();
        await handler.Handle(eventToPublish, tokenToPass);

        // Assert
        Assert.Equal(eventToPublish, passedEvent);
        Assert.Equal(tokenToPass, passedToken);
    }

    [Fact]
    public async Task LambdaWrappingRequestHandler_WrapsAction()
    {
        // Arrange
        TestRequest? passedRequest = null;
        var lambda = (TestRequest requestData) => { passedRequest = requestData; };
        var handler = new LambdaWrappingRequestHandler<TestRequest>(lambda);

        // Act
        var requestToExecute = new TestRequest("test-request");
        await handler.Handle(requestToExecute, CancellationToken.None);

        // Assert
        Assert.Equal(requestToExecute, passedRequest);
    }

    [Fact]
    public async Task LambdaWrappingRequestHandler_WrapsAsyncAction()
    {
        // Arrange
        TestRequest? passedRequest = null;
        var lambda = async (TestRequest requestData) =>
        {
            await Task.Delay(100);
            passedRequest = requestData;
        };
        var handler = new LambdaWrappingRequestHandler<TestRequest>(lambda);

        // Act
        var requestToExecute = new TestRequest("test-request");
        await handler.Handle(requestToExecute, CancellationToken.None);

        // Assert
        Assert.Equal(requestToExecute, passedRequest);
    }

    [Fact]
    public async Task LambdaWrappingRequestHandler_WrapsAsyncActionWithToken()
    {
        // Arrange
        TestRequest? passedRequest = null;
        CancellationToken? passedToken = null;
        var lambda = async (TestRequest requestData, CancellationToken token) =>
        {
            await Task.Delay(100, token);
            passedRequest = requestData;
            passedToken = token;
        };
        var handler = new LambdaWrappingRequestHandler<TestRequest>(lambda);

        // Act
        var requestToExecute = new TestRequest("test-request");
        var tokenToPass = new CancellationToken();
        await handler.Handle(requestToExecute, tokenToPass);

        // Assert
        Assert.Equal(requestToExecute, passedRequest);
        Assert.Equal(tokenToPass, passedToken);
    }

    [Fact]
    public async Task LambdaWrappingRequestWithResponseHandler_WrapsFunc()
    {
        // Arrange
        var expectedResult = "result";
        TestRequestWithResponse? passedRequest = null;
        var lambda = (TestRequestWithResponse requestData) => 
        { 
            passedRequest = requestData;
            return expectedResult;
        };
        var handler = new LambdaWrappingRequestHandler<TestRequestWithResponse, string>(lambda);

        // Act
        var requestToExecute = new TestRequestWithResponse("test-request");
        var result = await handler.Handle(requestToExecute, CancellationToken.None);

        // Assert
        Assert.Equal(requestToExecute, passedRequest);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task LambdaWrappingRequestWithResponseHandler_WrapsAsyncFunc()
    {
        // Arrange
        var expectedResult = "result";
        TestRequestWithResponse? passedRequest = null;
        var lambda = async (TestRequestWithResponse requestData) =>
        {
            await Task.Delay(100);
            passedRequest = requestData;
            return expectedResult;
        };
        var handler = new LambdaWrappingRequestHandler<TestRequestWithResponse, string>(lambda);

        // Act
        var requestToExecute = new TestRequestWithResponse("test-request");
        var result = await handler.Handle(requestToExecute, CancellationToken.None);

        // Assert
        Assert.Equal(requestToExecute, passedRequest);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task LambdaWrappingRequestWithResponseHandler_WrapsAsyncFuncWithToken()
    {
        // Arrange
        var expectedResult = "result";
        TestRequestWithResponse? passedRequest = null;
        CancellationToken? passedToken = null;
        var lambda = async (TestRequestWithResponse requestData, CancellationToken token) =>
        {
            await Task.Delay(100, token);
            passedRequest = requestData;
            passedToken = token;
            return expectedResult;
        };
        var handler = new LambdaWrappingRequestHandler<TestRequestWithResponse, string>(lambda);

        // Act
        var requestToExecute = new TestRequestWithResponse("test-request");
        var tokenToPass = new CancellationToken();
        var result = await handler.Handle(requestToExecute, tokenToPass);

        // Assert
        Assert.Equal(requestToExecute, passedRequest);
        Assert.Equal(tokenToPass, passedToken);
        Assert.Equal(expectedResult, result);
    }
}
