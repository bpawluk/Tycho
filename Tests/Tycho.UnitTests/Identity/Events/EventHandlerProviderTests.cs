using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Identity.Events;

public sealed class EventHandlerProviderTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IEventHandler<TestEvent> _registeredHandler;
    private readonly IEventHandler<OtherEvent> _registeredOtherEventHandler;
    private readonly EventHandlerIdentity _registeredHandlerId;

    private readonly EventHandlerProvider _sut;

    public EventHandlerProviderTests()
    {
        var services = new ServiceCollection();

        var firstEventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        IEventHandler<TestEvent> firstEventHandler = firstEventHandlerMock.Object;
        var firstEventHandlerId = EventHandlerIdentity.Create<TestEventOtherHandler>();
        services.AddKeyedSingleton(firstEventHandlerId, firstEventHandler);

        var secondEventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        _registeredHandler = secondEventHandlerMock.Object;
        _registeredHandlerId = EventHandlerIdentity.Create<TestEventHandler>();
        services.AddKeyedSingleton(_registeredHandlerId, _registeredHandler);

        _registeredOtherEventHandler = Mock.Of<IEventHandler<OtherEvent>>();
        services.AddKeyedSingleton(_registeredHandlerId, _registeredOtherEventHandler);

        _serviceProvider = services.BuildServiceProvider();
        _sut = new EventHandlerProvider(_serviceProvider);
    }

    [Fact]
    public void GetHandler_WithRegisteredHandler_ReturnsTheHandler()
    {
        // Act
        IEventHandler<TestEvent> result = _sut.GetHandler<TestEvent>(_registeredHandlerId);

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

    [Fact]
    public void GetHandler_WithSharedIdForDifferentEvents_ReturnsMatchingEventHandler()
    {
        // Act
        IEventHandler<OtherEvent> result = _sut.GetHandler<OtherEvent>(_registeredHandlerId);

        // Assert
        Assert.Same(_registeredOtherEventHandler, result);
    }

    [Fact]
    public void GetHandler_WithIdRegisteredForAnotherEvent_ThrowsArgumentException()
    {
        // Act
        void Act() => _sut.GetHandler<AnotherEvent>(_registeredHandlerId);

        // Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(Act);
        Assert.Equal("handlerId", exception.ParamName);
    }

    [Fact]
    public void GetHandler_WithMultipleFactories_CreatesOnlyRequestedHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        var requestedFactoryMock = new Mock<Func<IEventHandler<TestEvent>>>(MockBehavior.Strict);
        var unrelatedFactoryMock = new Mock<Func<IEventHandler<TestEvent>>>(MockBehavior.Strict);
        requestedFactoryMock.Setup(factory => factory()).Returns(_registeredHandler);
        services.AddKeyedScoped<IEventHandler<TestEvent>>(_registeredHandlerId, (_, _) => requestedFactoryMock.Object());
        services.AddKeyedScoped<IEventHandler<TestEvent>>(
            EventHandlerIdentity.Create<TestEventOtherHandler>(), (_, _) => unrelatedFactoryMock.Object());
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        var sut = new EventHandlerProvider(scope.ServiceProvider);

        // Act
        IEventHandler<TestEvent> result = sut.GetHandler<TestEvent>(_registeredHandlerId);

        // Assert
        Assert.Same(_registeredHandler, result);
        requestedFactoryMock.Verify(factory => factory(), Times.Once);
        unrelatedFactoryMock.VerifyNoOtherCalls();
    }

    public void Dispose() => _serviceProvider.Dispose();
}
