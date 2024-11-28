using Moq;
using Tycho.Events;
using Tycho.Events.Handling;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Sources;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Routing.Sources;

public class UpStreamMappedHandlersSourceTests
{
    private readonly IEventHandler<OtherEvent> _handlerFromParent = new OtherEventHandler();

    private readonly HandlerIdentity[] _identitiesFromParent =
    [
        new(typeof(OtherEvent), typeof(OtherEventHandler), typeof(TestModule)),
        new(typeof(OtherEvent), typeof(OtherEventHandler), typeof(OtherModule))
    ];

    private readonly UpStreamMappedHandlersSource<TestEvent, OtherEvent> _sut;

    public UpStreamMappedHandlersSourceTests()
    {
        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        mapMock.Setup(m => m(It.IsAny<TestEvent>()))
               .Returns(new OtherEvent());

        var eventRouterMock = new Mock<IEventRouter>();
        eventRouterMock.Setup(m => m.IdentifyHandlers<OtherEvent>())
                       .Returns(_identitiesFromParent);
        eventRouterMock.Setup(m => m.FindHandler(It.Is<HandlerIdentity>(
                           id => id.MatchesEvent(typeof(OtherEvent)))))
                       .Returns(_handlerFromParent);

        var parentMock = new Mock<IParent>();
        parentMock.SetupGet(p => p.EventRouter)
                  .Returns(eventRouterMock.Object);

        _sut = new UpStreamMappedHandlersSource<TestEvent, OtherEvent>(
            parentMock.Object, 
            mapMock.Object);
    }

    [Fact]
    public void IdentifyHandlers_ForTestEvent_ReturnsHandlersFromSubmodule()
    {
        // Arrange
        var expectedIdentities = _identitiesFromParent
            .Select(id => id.ForEvent(typeof(TestEvent)))
            .ToArray();

        // Act
        var result = _sut.IdentifyHandlers<TestEvent>();

        // Assert
        Assert.Equal(expectedIdentities, result);
    }

    [Fact]
    public void IdentifyHandlers_ForOtherEvent_ReturnsEmptyCollection()
    {
        // Arrange
        // - no arrangement required

        // Act
        var result = _sut.IdentifyHandlers<OtherEvent>();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindHandler_ForIdentityOfTestEventHandler_ReturnsHandlerFromSubmodule()
    {
        // Arrange
        var handlerIdentity = new HandlerIdentity(typeof(TestEvent), typeof(OtherEventHandler), typeof(TestModule));

        // Act
        var result = _sut.FindHandler(handlerIdentity);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<WrappingEventHandler<TestEvent, OtherEvent>>(result);
    }

    [Fact]
    public void FindHandler_ForIdentityOfOtherEventHandler_ReturnsNull()
    {
        // Arrange
        var handlerIdentity = new HandlerIdentity(typeof(OtherEvent), typeof(OtherEventHandler), typeof(TestModule));

        // Act
        var result = _sut.FindHandler(handlerIdentity);

        // Assert
        Assert.Null(result);
    }
}