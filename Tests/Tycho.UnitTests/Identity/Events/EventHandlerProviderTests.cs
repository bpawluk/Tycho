using Moq;
using Tycho.Events;
using Tycho.Identity.Events;
using Tycho.Events.Registrating.Registrations;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Tycho.UnitTests.Identity.Events;

public class EventHandlerProviderTests
{
    private readonly IEventHandler<TestEvent> _registeredHandler;
    private readonly EventHandlerIdentity _registeredHandlerId;

    private readonly EventHandlerProvider _sut;

    public EventHandlerProviderTests()
    {
        var internals = new Internals(typeof(TestModule));

        var firstEventRegistrationMock = new Mock<IFinalEventRegistration<TestEvent>>();
        var firstEventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        var firstEventHandler = firstEventHandlerMock.Object;
        firstEventRegistrationMock.SetupGet(r => r.Handler).Returns(firstEventHandler);
        var firstEventHandlerId = EventHandlerIdentity.Create<TestEventOtherHandler, TestEvent>();
        firstEventRegistrationMock.SetupGet(r => r.HandlerId).Returns(firstEventHandlerId);
        internals.GetServiceCollection().AddSingleton(firstEventRegistrationMock.Object);

        var secondEventRegistrationMock = new Mock<IFinalEventRegistration<TestEvent>>();
        var secondEventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        _registeredHandler = secondEventHandlerMock.Object;
        secondEventRegistrationMock.SetupGet(r => r.Handler).Returns(_registeredHandler);
        _registeredHandlerId = EventHandlerIdentity.Create<TestEventHandler, TestEvent>();
        secondEventRegistrationMock.SetupGet(r => r.HandlerId).Returns(_registeredHandlerId);
        internals.GetServiceCollection().AddSingleton(secondEventRegistrationMock.Object);

        var thirdEventRegistrationMock = new Mock<IFinalEventRegistration<OtherEvent>>();
        var thirdEventHandlerMock = new Mock<IEventHandler<OtherEvent>>();
        var thirdEventHandler = thirdEventHandlerMock.Object;
        thirdEventRegistrationMock.SetupGet(r => r.Handler).Returns(thirdEventHandler);
        var thirdEventHandlerId = EventHandlerIdentity.Create<OtherEventHandler, OtherEvent>();
        thirdEventRegistrationMock.SetupGet(r => r.HandlerId).Returns(thirdEventHandlerId);
        internals.GetServiceCollection().AddSingleton(thirdEventRegistrationMock.Object);

        internals.Build();
        _sut = new EventHandlerProvider(internals);
    }

    [Fact]
    public void GetHandler_WithRegisteredHandler_ReturnsTheHandler()
    {
        // Act
        var result = _sut.GetHandler<TestEvent>(_registeredHandlerId);

        // Assert
        Assert.Same(_registeredHandler, result);
    }

    [Fact]
    public void GetHandler_WithMissingHandler_ThrowsArgumentException()
    {
        // Arrange
        var missingHandlerId = EventHandlerIdentity.Create<TestEventAnotherHandler, TestEvent>();

        // Act 
        void Act() => _sut.GetHandler<TestEvent>(missingHandlerId);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}
