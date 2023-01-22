using System;
using System.Threading;
using Test.Utils;
using Tycho;
using Tycho.Messaging;

namespace Test.Structure;

public abstract class BaseModuleTests
{
    protected readonly Mock<IMessageBroker> _messageBrokerMock;
    protected readonly IModule _module;

    public BaseModuleTests()
    {
        _messageBrokerMock = new Mock<IMessageBroker>();
        _module = CreateModuleUnderTest();
    }

    [Fact]
    public void SetBroker_BrokerAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        SetBroker();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => SetBroker());
    }

    [Fact]
    public void PublishEvent_NoMessageBroker_ThrowsInvalidOperationException()
    {
        // Arrange
        var eventToPublish = new TestEvent("test-event");
        var tokenToPass = new CancellationToken();

        // Act
        Assert.Throws<InvalidOperationException>(() => _module.Publish(eventToPublish, tokenToPass));

        // Assert
        _messageBrokerMock.Verify(broker => broker.Publish(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()),  Times.Never());
    }

    [Fact]
    public void PublishEvent_MessageBrokerExists_InvokesTheBroker()
    {
        // Arrange
        SetBroker();
        var eventToPublish = new TestEvent("test-event");
        var tokenToPass = new CancellationToken();

        // Act
        _module.Publish(eventToPublish, tokenToPass);

        // Assert
        _messageBrokerMock.Verify(broker => broker.Publish(eventToPublish, tokenToPass), Times.Once());
    }

    [Fact]
    public void ExecuteCommand_NoMessageBroker_ThrowsInvalidOperationException()
    {
        // Arrange
        var commandToExecute = new TestCommand("test-command");
        var tokenToPass = new CancellationToken();

        // Act
        Assert.ThrowsAsync<InvalidOperationException>(() => _module.Execute(commandToExecute, tokenToPass));

        // Assert
        _messageBrokerMock.Verify(broker => broker.Execute(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public void ExecuteCommand_MessageBrokerExists_InvokesTheBroker()
    {
        // Arrange
        SetBroker();
        var commandToExecute = new TestCommand("test-command");
        var tokenToPass = new CancellationToken();

        // Act
        _module.Execute(commandToExecute, tokenToPass);

        // Assert
        _messageBrokerMock.Verify(broker => broker.Execute(commandToExecute, tokenToPass), Times.Once());
    }

    [Fact]
    public void ExecuteQuery_NoMessageBroker_ThrowsInvalidOperationException()
    {
        // Arrange
        var queryToExecute = new TestQuery("test-query");
        var tokenToPass = new CancellationToken();

        // Act
        Assert.ThrowsAsync<InvalidOperationException>(() => _module.Execute<TestQuery, string>(queryToExecute, tokenToPass));

        // Assert
        _messageBrokerMock.Verify(broker => broker.Execute<TestQuery, string>(It.IsAny<TestQuery>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public void ExecuteQuery_MessageBrokerExists_InvokesTheBroker()
    {
        // Arrange
        SetBroker();
        var queryToExecute = new TestQuery("test-query");
        var tokenToPass = new CancellationToken();

        // Act
        _module.Execute<TestQuery, string>(queryToExecute, tokenToPass);

        // Assert
        _messageBrokerMock.Verify(broker => broker.Execute<TestQuery, string>(queryToExecute, tokenToPass), Times.Once());
    }

    protected abstract IModule CreateModuleUnderTest();

    protected abstract void SetBroker();
}
