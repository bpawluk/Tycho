using System;
using System.Threading;
using Tycho;
using Tycho.Messaging;
using UnitTests.Utils;

namespace UnitTests.Structure;

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
    public void ExecuteRequest_NoMessageBroker_ThrowsInvalidOperationException()
    {
        // Arrange
        var requestToExecute = new TestRequest("test-request");
        var tokenToPass = new CancellationToken();

        // Act
        Assert.ThrowsAsync<InvalidOperationException>(() => _module.Execute(requestToExecute, tokenToPass));

        // Assert
        _messageBrokerMock.Verify(broker => broker.Execute(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public void ExecuteRequest_MessageBrokerExists_InvokesTheBroker()
    {
        // Arrange
        SetBroker();
        var requestToExecute = new TestRequest("test-request");
        var tokenToPass = new CancellationToken();

        // Act
        _module.Execute(requestToExecute, tokenToPass);

        // Assert
        _messageBrokerMock.Verify(broker => broker.Execute(requestToExecute, tokenToPass), Times.Once());
    }

    [Fact]
    public void ExecuteRequestWithResponse_NoMessageBroker_ThrowsInvalidOperationException()
    {
        // Arrange
        var requestToExecute = new TestRequestWithResponse("test-request");
        var tokenToPass = new CancellationToken();

        // Act
        Assert.ThrowsAsync<InvalidOperationException>(() => _module.Execute<TestRequestWithResponse, string>(requestToExecute, tokenToPass));

        // Assert
        _messageBrokerMock.Verify(broker => broker.Execute<TestRequestWithResponse, string>(It.IsAny<TestRequestWithResponse>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public void ExecuteRequestWithResponse_MessageBrokerExists_InvokesTheBroker()
    {
        // Arrange
        SetBroker();
        var requestToExecute = new TestRequestWithResponse("test-request");
        var tokenToPass = new CancellationToken();

        // Act
        _module.Execute<TestRequestWithResponse, string>(requestToExecute, tokenToPass);

        // Assert
        _messageBrokerMock.Verify(broker => broker.Execute<TestRequestWithResponse, string>(requestToExecute, tokenToPass), Times.Once());
    }

    protected abstract IModule CreateModuleUnderTest();

    protected abstract void SetBroker();
}
