using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events;
using Tycho.Events.Registrating.Registrations;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Identity.Events;

public class EventHandlerProviderTests
{
    private readonly IEventHandler<TestEvent> _registeredHandler;
    private readonly EventHandlerIdentity _registeredHandlerId;

    private readonly EventHandlerProvider _sut;

    public EventHandlerProviderTests()
    {
        var services = new ServiceCollection();

        var firstEventRegistrationMock = new Mock<IFinalEventRegistration<TestEvent>>();
        services.AddSingleton(firstEventRegistrationMock.Object);
        
        var firstEventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        var firstEventHandler = firstEventHandlerMock.Object;
        firstEventRegistrationMock.SetupGet(r => r.Handler).Returns(firstEventHandler);
        
        var firstEventHandlerId = EventHandlerIdentity.Create<TestEventOtherHandler>();
        firstEventRegistrationMock.SetupGet(r => r.HandlerId).Returns(firstEventHandlerId);

        var secondEventRegistrationMock = new Mock<IFinalEventRegistration<TestEvent>>();
        services.AddSingleton(secondEventRegistrationMock.Object);
        
        var secondEventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        _registeredHandler = secondEventHandlerMock.Object;
        secondEventRegistrationMock.SetupGet(r => r.Handler).Returns(_registeredHandler);
        
        _registeredHandlerId = EventHandlerIdentity.Create<TestEventHandler>();
        secondEventRegistrationMock.SetupGet(r => r.HandlerId).Returns(_registeredHandlerId);

        var serviceProvider = services.BuildServiceProvider();
        _sut = new EventHandlerProvider(serviceProvider);
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
        var missingHandlerId = EventHandlerIdentity.Create<TestEventAnotherHandler>();

        // Act 
        void Act() => _sut.GetHandler<TestEvent>(missingHandlerId);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}
