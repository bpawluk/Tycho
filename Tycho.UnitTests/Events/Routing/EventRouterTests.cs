using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events;
using Tycho.Events.Routing;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Routing;

public class EventRouterTests
{
    private readonly HandlerIdentity[] _firstSourceHandlers =
    [
        new HandlerIdentity(
            typeof(TestEvent),
            typeof(TestEventHandler),
            typeof(TestModule))
    ];

    private readonly HandlerIdentity[] _secondSourceHandlers =
    [
        new HandlerIdentity(
            typeof(TestEvent),
            typeof(TestEventOtherHandler),
            typeof(TestModule)),
        new HandlerIdentity(
            typeof(TestEvent),
            typeof(TestEventAnotherHandler),
            typeof(TestModule))
    ];

    private readonly HandlerIdentity[] _thirdSourceHandlers =
    [
        new HandlerIdentity(
            typeof(OtherEvent),
            typeof(OtherEventHandler),
            typeof(TestModule))
    ];

    private readonly EventRouter _sut;

    public EventRouterTests()
    {
        var internals = new Internals(typeof(TestModule));
        var services = internals.GetServiceCollection();

        var firstHandlersSourceMock = new Mock<IHandlersSource>();
        firstHandlersSourceMock.Setup(source => source.IdentifyHandlers<TestEvent>())
                               .Returns(_firstSourceHandlers);
        firstHandlersSourceMock.Setup(source => source.IdentifyHandlers<OtherEvent>())
                               .Returns([]);
        firstHandlersSourceMock.Setup(source => source.IdentifyHandlers<AnotherEvent>())
                               .Returns([]);
        firstHandlersSourceMock.Setup(source => source.FindHandler(It.IsAny<HandlerIdentity>()))
                               .Returns((HandlerIdentity identity) => FindHandlerMock(
                                   _firstSourceHandlers,
                                   identity));

        var secondHandlersSourceMock = new Mock<IHandlersSource>();
        secondHandlersSourceMock.Setup(source => source.IdentifyHandlers<TestEvent>())
                                .Returns(_secondSourceHandlers);
        secondHandlersSourceMock.Setup(source => source.IdentifyHandlers<OtherEvent>())
                                .Returns([]);
        secondHandlersSourceMock.Setup(source => source.IdentifyHandlers<AnotherEvent>())
                               .Returns([]);
        secondHandlersSourceMock.Setup(source => source.FindHandler(It.IsAny<HandlerIdentity>()))
                               .Returns((HandlerIdentity identity) => FindHandlerMock(
                                   _secondSourceHandlers,
                                   identity));

        var thirdHandlersSourceMock = new Mock<IHandlersSource>();
        thirdHandlersSourceMock.Setup(source => source.IdentifyHandlers<TestEvent>())
                               .Returns([]);
        thirdHandlersSourceMock.Setup(source => source.IdentifyHandlers<OtherEvent>())
                               .Returns(_thirdSourceHandlers);
        thirdHandlersSourceMock.Setup(source => source.IdentifyHandlers<AnotherEvent>())
                               .Returns([]);
        thirdHandlersSourceMock.Setup(source => source.FindHandler(It.IsAny<HandlerIdentity>()))
                               .Returns((HandlerIdentity identity) => FindHandlerMock(
                                   _thirdSourceHandlers,
                                   identity));

        services.AddTransient(_ => firstHandlersSourceMock.Object);
        services.AddTransient(_ => secondHandlersSourceMock.Object);
        services.AddTransient(_ => thirdHandlersSourceMock.Object);
        internals.Build();
        _sut = new EventRouter(internals);
    }

    [Fact]
    public void IdentifyHandlers_ForHandledEvent_ReturnsHandlersFromAllSources()
    {
        // Arrange
        var expectedIdentities = _firstSourceHandlers
            .Concat(_secondSourceHandlers)
            .ToArray();

        // Act
        var result = _sut.IdentifyHandlers<TestEvent>();

        // Assert
        Assert.Equal(expectedIdentities, result);
    }

    [Fact]
    public void IdentifyHandlers_ForUnhandledEvent_ReturnsEmptyCollection()
    {
        // Arrange
        // - no arrangement required

        // Act
        var result = _sut.IdentifyHandlers<AnotherEvent>();

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(typeof(TestEventHandler))]
    [InlineData(typeof(TestEventOtherHandler))]
    [InlineData(typeof(TestEventAnotherHandler))]
    public void FindHandler_ForHandledEvent_ReturnsFirstMatchingHandler(Type handlerType)
    {
        // Arrange
        var identity = new HandlerIdentity(typeof(TestEvent), handlerType, typeof(TestModule));

        // Act
        var result = _sut.FindHandler(identity);

        // Assert
        Assert.NotNull(result);
        Assert.IsType(handlerType, result);
    }

    [Fact]
    public void FindHandler_ForUnhandledEvent_ReturnsNull()
    {
        // Arrange
        var identity = new HandlerIdentity(typeof(AnotherEvent), typeof(AnotherEventHandler), typeof(TestModule));

        // Act
        var result = _sut.FindHandler(identity);

        // Assert
        Assert.Null(result);
    }

    private static IEventHandler? FindHandlerMock(HandlerIdentity[] allIds, HandlerIdentity searchedId)
    {
        if (allIds.Contains(searchedId))
        {
            Type handlerType = typeof(object);

            if (searchedId.MatchesHandler(typeof(TestEventHandler)))
            {
                handlerType = typeof(TestEventHandler);
            }

            if (searchedId.MatchesHandler(typeof(TestEventOtherHandler)))
            {
                handlerType = typeof(TestEventOtherHandler);
            }

            if (searchedId.MatchesHandler(typeof(TestEventAnotherHandler)))
            {
                handlerType = typeof(TestEventAnotherHandler);
            }

            return Activator.CreateInstance(handlerType) as IEventHandler;
        }
        return null;
    }
}
