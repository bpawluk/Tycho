using Moq;
using Tycho.Events;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Sources;
using Tycho.Modules;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Routing.Sources;

public class DownStreamHandlersSourceTests
{
    private readonly IEventHandler<TestEvent> _expectedHandler = new TestEventHandler();
    private readonly HandlerIdentity[] _expectedIdentities =
    [
        new HandlerIdentity("some-event", "some-handler", "some-module"),
        new HandlerIdentity("other-event", "other-handler", "other-module")
    ];

    private readonly DownStreamHandlersSource<TestEvent, TestModule> _sut;

    public DownStreamHandlersSourceTests()
    {
        var eventRouterMock = new Mock<IEventRouter>();
        eventRouterMock.Setup(m => m.IdentifyHandlers<TestEvent>())
                       .Returns(_expectedIdentities);
        eventRouterMock.Setup(m => m.FindHandler(It.IsAny<HandlerIdentity>()))
                       .Returns(_expectedHandler);

        var submoduleMock = new Mock<IModule<TestModule>>();
        submoduleMock.SetupGet(m => m.EventRouter)
                      .Returns(eventRouterMock.Object);

        _sut = new DownStreamHandlersSource<TestEvent, TestModule>(submoduleMock.Object);
    }

    [Fact]
    public void IdentifyHandlers_ForTestEvent_ReturnsHandlersFromSubmodule()
    {
        // Arrange
        // - no arrangements required

        // Act
        var result = _sut.IdentifyHandlers<TestEvent>();

        // Assert
        Assert.Equal(_expectedIdentities, result);
    }

    [Fact]
    public void IdentifyHandlers_ForOtherEvent_ReturnsEmptyCollection()
    {
        // Arrange
        // - no arrangements required

        // Act
        var result = _sut.IdentifyHandlers<OtherEvent>();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindHandler_ForIdentityOfTestEventHandler_ReturnsHandlerFromSubmodule()
    {
        // Arrange
        var handlerIdentity = new HandlerIdentity(typeof(TestEvent), typeof(object), typeof(object));

        // Act
        var result = _sut.FindHandler(handlerIdentity);

        // Assert
        Assert.Equal(_expectedHandler, result);
    }

    [Fact]
    public void FindHandler_ForIdentityOfOtherEventHandler_ReturnsNull()
    {
        // Arrange
        var handlerIdentity = new HandlerIdentity(typeof(OtherEvent), typeof(object), typeof(object));

        // Act
        var result = _sut.FindHandler(handlerIdentity);

        // Assert
        Assert.Null(result);
    }
}
