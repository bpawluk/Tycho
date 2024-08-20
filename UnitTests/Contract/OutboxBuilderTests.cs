﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Utils;
using Tycho.DependencyInjection;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;
using Tycho.Contract.Outbox.Builder;

namespace UnitTests.Contract;

public class OutboxBuilderTests
{
    private readonly OutboxBuilder _outboxBuilder;

    public OutboxBuilderTests()
    {
        var instanceCreatorMock = new Mock<IInstanceCreator>();
        var messageRouterMock = new MockMessageRouter();
        _outboxBuilder = new OutboxBuilder(instanceCreatorMock.Object, messageRouterMock);
    }

    [Fact]
    public void PublishesEvent_EventAlreadyDefined_ThrowsArgumentException()
    {
        // Arrange
        _outboxBuilder.Publishes<TestEvent>();

        // Act & Assert
        Assert.Throws<ArgumentException>(_outboxBuilder.Publishes<TestEvent>);
    }

    [Fact]
    public void SendsCommand_CommandAlreadyDefined_ThrowsArgumentException()
    {
        // Arrange
        _outboxBuilder.Sends<TestCommand>();

        // Act & Assert
        Assert.Throws<ArgumentException>(_outboxBuilder.Sends<TestCommand>);
    }

    [Fact]
    public void SendsQuery_QueryAlreadyDefined_ThrowsArgumentException()
    {
        // Arrange
        _outboxBuilder.Sends<TestQuery, string>();

        // Act & Assert
        Assert.Throws<ArgumentException>(_outboxBuilder.Sends<TestQuery, string>);
    }

    [Fact]
    public void PassOn_EventNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.ForwardEvent<TestEvent, TestModule>);
        Assert.Throws<InvalidOperationException>(
            () => _outboxBuilder.ForwardEvent<TestEvent, OtherEvent, TestModule>(eventData => new(int.MinValue)));
    }

    [Fact]
    public void ExposeEvent_EventNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.ExposeEvent<TestEvent>);
        Assert.Throws<InvalidOperationException>(
            () => _outboxBuilder.ExposeEvent<TestEvent, OtherEvent>(eventData => new(int.MinValue)));
    }

    [Fact]
    public void HandleEvent_EventNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _outboxBuilder.HandleEvent<TestEvent>(_ => { }));
    }

    [Fact]
    public void Forward_CommandNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.ForwardCommand<TestCommand, TestModule>);
        Assert.Throws<InvalidOperationException>(
            () => _outboxBuilder.ForwardCommand<TestCommand, OtherCommand, TestModule>(commandData => new(int.MinValue)));
    }

    [Fact]
    public void ExposeCommand_CommandNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.ExposeCommand<TestCommand>);
        Assert.Throws<InvalidOperationException>(
            () => _outboxBuilder.ExposeCommand<TestCommand, OtherCommand>(commandData => new(int.MinValue)));
    }

    [Fact]
    public void HandleCommand_CommandNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _outboxBuilder.HandleCommand<TestCommand>(_ => Task.CompletedTask));
    }

    [Fact]
    public void Forward_QueryNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.ForwardQuery<TestQuery, string, TestModule>);
        Assert.Throws<InvalidOperationException>(
            () => _outboxBuilder.ForwardQuery<TestQuery, string, OtherTestQuery, string, TestModule>(
                queryData => new(queryData.Name), response => response));
        Assert.Throws<InvalidOperationException>(
            () => _outboxBuilder.ForwardQuery<TestQuery, string, OtherQuery, object, TestModule>(
                commandData => new(int.MinValue), response => response.ToString()!));
    }

    [Fact]
    public void ExposeQuery_QueryNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.ExposeQuery<TestQuery, string>);
        Assert.Throws<InvalidOperationException>(
            () => _outboxBuilder.ExposeQuery<TestQuery, string, OtherTestQuery, string>(
                queryData => new(queryData.Name), response => response));
        Assert.Throws<InvalidOperationException>(
            () => _outboxBuilder.ExposeQuery<TestQuery, string, OtherQuery, object>(
                commandData => new(int.MinValue), response => response.ToString()!));
    }

    [Fact]
    public void HandleQuery_QueryNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.HandleQuery<TestQuery, string, TestMessageHandler>);
    }

    [Fact]
    public void Build_AllHandlersMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        _outboxBuilder.Publishes<TestEvent>();
        _outboxBuilder.Sends<TestCommand>();
        _outboxBuilder.Sends<TestQuery, string>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.Build);
    }

    [Fact]
    public void Build_QueryHandlerMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        _outboxBuilder.Publishes<TestEvent>();
        _outboxBuilder.HandleEvent<TestEvent>(_ => { });

        _outboxBuilder.Sends<TestCommand>();
        _outboxBuilder.HandleCommand<TestCommand, TestMessageHandler>();

        _outboxBuilder.Sends<TestQuery, string>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.Build);
    }

    [Fact]
    public void Build_CommandHandlerMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        _outboxBuilder.Publishes<TestEvent>();
        _outboxBuilder.HandleEvent<TestEvent>((_, _) => Task.CompletedTask);

        _outboxBuilder.Sends<TestCommand>();

        _outboxBuilder.Sends<TestQuery, string>();
        _outboxBuilder.HandleQuery<TestQuery, string>(_ => Task.FromResult("test-response"));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.Build);
    }

    [Fact]
    public async Task Build_EventHandlerMissing_ReturnsCorrectMessageBroker()
    {
        // Arrange
        _outboxBuilder.Publishes<TestEvent>();

        var commandHandler = new TestMessageHandler();
        _outboxBuilder.Sends<TestCommand>();
        _outboxBuilder.HandleCommand(commandHandler);

        var queryHandler = new TestMessageHandler();
        _outboxBuilder.Sends<TestQuery, string>();
        _outboxBuilder.HandleQuery(queryHandler);

        // Act
        var broker = _outboxBuilder.Build();
        broker.Publish(new TestEvent("test-event"));
        await broker.Execute(new TestCommand("test-command"));
        var queryResponse = await broker.Execute<TestQuery, string>(new TestQuery("test-query"));

        // Assert
        Assert.True(commandHandler.HandlerCalled);
        Assert.True(queryHandler.HandlerCalled);
        Assert.Equal(queryHandler.QueryResponse, queryResponse);
    }

    [Fact]
    public async Task Build_AllMessagesHandled_ReturnsCorrectMessageBroker()
    {
        // Arrange
        var eventHandler = new TestMessageHandler();
        _outboxBuilder.Publishes<TestEvent>();
        _outboxBuilder.HandleEvent(eventHandler);

        var commandHandler = new TestMessageHandler();
        _outboxBuilder.Sends<TestCommand>();
        _outboxBuilder.HandleCommand(commandHandler);

        var queryHandler = new TestMessageHandler();
        _outboxBuilder.Sends<TestQuery, string>();
        _outboxBuilder.HandleQuery(queryHandler);

        // Act
        var broker = _outboxBuilder.Build();
        broker.Publish(new TestEvent("test-event"));
        await broker.Execute(new TestCommand("test-command"));
        var queryResponse = await broker.Execute<TestQuery, string>(new TestQuery("test-query"));

        // Assert
        Assert.True(eventHandler.HandlerCalled);
        Assert.True(commandHandler.HandlerCalled);
        Assert.True(queryHandler.HandlerCalled);
        Assert.Equal(queryHandler.QueryResponse, queryResponse);
    }

    [Fact]
    public void Build_NoMessagesDefined_ReturnsCorrectMessageBroker()
    {
        // Arrange
        // - no arrangement required

        // Act
        var broker = _outboxBuilder.Build();

        // Assert
        Assert.NotNull(broker);
    }

    private class MockMessageRouter : IMessageRouter
    {
        private readonly Dictionary<Type, object> _handlers = new();

        IEnumerable<IEventHandler<Event>> IMessageRouter.GetEventHandlers<Event>()
        {
            if (_handlers.ContainsKey(typeof(Event)))
            {
                return new[] { (_handlers[typeof(Event)] as IEventHandler<Event>)! };
            }
            return Array.Empty<IEventHandler<Event>>();
        }

        IRequestHandler<Command> IMessageRouter.GetRequestHandler<Command>() =>
            (_handlers[typeof(Command)] as IRequestHandler<Command>)!;

        IRequestHandler<Query, Response> IMessageRouter.GetRequestWithResponseHandler<Query, Response>() =>
            (_handlers[typeof(Query)] as IRequestHandler<Query, Response>)!;

        void IMessageRouter.RegisterEventHandler<Event>(IEventHandler<Event> eventHandler) =>
            _handlers[typeof(Event)] = eventHandler;

        void IMessageRouter.RegisterRequestHandler<Command>(IRequestHandler<Command> commandHandler) =>
            _handlers[typeof(Command)] = commandHandler;

        void IMessageRouter.RegisterRequestWithResponseHandler<Query, Response>(IRequestHandler<Query, Response> queryHandler) =>
            _handlers[typeof(Query)] = queryHandler;
    }
}
