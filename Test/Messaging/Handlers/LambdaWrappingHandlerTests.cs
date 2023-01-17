using System.Threading;
using System.Threading.Tasks;
using Test.Utils;
using Tycho.Messaging.Handlers;

namespace Test.Messaging.Handlers;

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
    public async Task LambdaWrappingCommandHandler_WrapsAction()
    {
        // Arrange
        TestCommand? passedCommand = null;
        var lambda = (TestCommand commandData) => { passedCommand = commandData; };
        var handler = new LambdaWrappingCommandHandler<TestCommand>(lambda);

        // Act
        var commandToExecute = new TestCommand("test-command");
        await handler.Handle(commandToExecute, CancellationToken.None);

        // Assert
        Assert.Equal(commandToExecute, passedCommand);
    }

    [Fact]
    public async Task LambdaWrappingCommandHandler_WrapsAsyncAction()
    {
        // Arrange
        TestCommand? passedCommand = null;
        var lambda = async (TestCommand commandData) =>
        {
            await Task.Delay(100);
            passedCommand = commandData;
        };
        var handler = new LambdaWrappingCommandHandler<TestCommand>(lambda);

        // Act
        var commandToExecute = new TestCommand("test-command");
        await handler.Handle(commandToExecute, CancellationToken.None);

        // Assert
        Assert.Equal(commandToExecute, passedCommand);
    }

    [Fact]
    public async Task LambdaWrappingCommandHandler_WrapsAsyncActionWithToken()
    {
        // Arrange
        TestCommand? passedCommand = null;
        CancellationToken? passedToken = null;
        var lambda = async (TestCommand commandData, CancellationToken token) =>
        {
            await Task.Delay(100, token);
            passedCommand = commandData;
            passedToken = token;
        };
        var handler = new LambdaWrappingCommandHandler<TestCommand>(lambda);

        // Act
        var commandToExecute = new TestCommand("test-command");
        var tokenToPass = new CancellationToken();
        await handler.Handle(commandToExecute, tokenToPass);

        // Assert
        Assert.Equal(commandToExecute, passedCommand);
        Assert.Equal(tokenToPass, passedToken);
    }

    [Fact]
    public async Task LambdaWrappingQueryHandler_WrapsFunc()
    {
        // Arrange
        var expectedResult = "result";
        TestQuery? passedQuery = null;
        var lambda = (TestQuery queryData) => 
        { 
            passedQuery = queryData;
            return expectedResult;
        };
        var handler = new LambdaWrappingQueryHandler<TestQuery, string>(lambda);

        // Act
        var queryToExecute = new TestQuery("test-query");
        var result = await handler.Handle(queryToExecute, CancellationToken.None);

        // Assert
        Assert.Equal(queryToExecute, passedQuery);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task LambdaWrappingQueryHandler_WrapsAsyncFunc()
    {
        // Arrange
        var expectedResult = "result";
        TestQuery? passedQuery = null;
        var lambda = async (TestQuery queryData) =>
        {
            await Task.Delay(100);
            passedQuery = queryData;
            return expectedResult;
        };
        var handler = new LambdaWrappingQueryHandler<TestQuery, string>(lambda);

        // Act
        var queryToExecute = new TestQuery("test-query");
        var result = await handler.Handle(queryToExecute, CancellationToken.None);

        // Assert
        Assert.Equal(queryToExecute, passedQuery);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task LambdaWrappingQueryHandler_WrapsAsyncFuncWithToken()
    {
        // Arrange
        var expectedResult = "result";
        TestQuery? passedQuery = null;
        CancellationToken? passedToken = null;
        var lambda = async (TestQuery queryData, CancellationToken token) =>
        {
            await Task.Delay(100, token);
            passedQuery = queryData;
            passedToken = token;
            return expectedResult;
        };
        var handler = new LambdaWrappingQueryHandler<TestQuery, string>(lambda);

        // Act
        var queryToExecute = new TestQuery("test-query");
        var tokenToPass = new CancellationToken();
        var result = await handler.Handle(queryToExecute, tokenToPass);

        // Assert
        Assert.Equal(queryToExecute, passedQuery);
        Assert.Equal(tokenToPass, passedToken);
        Assert.Equal(expectedResult, result);
    }
}
