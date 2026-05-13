using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events.Broker;
using Tycho.Events.Delivery;
using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Broker;

public class ScopedEventBrokerTests
{
    [Fact]
    public void Route_WithNoRegistrations_ReturnsEmpty()
    {
        // Arrange
        var sut = CreateSut(_ => { });

        // Act
        var result = sut.Route(Guid.NewGuid(), new TestEvent());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Route_WithMultipleRegistrations_ReturnsAllRoutedEvents()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventPayload = new TestEvent();

        var emptyRegistration = new Mock<IEventRegistration<TestEvent>>();
        emptyRegistration.Setup(r => r.Route(It.IsAny<Guid>(), It.IsAny<TestEvent>()))
                         .Returns([]);

        var firstRoutedEvent = CreateRoutedEvent();
        var firstRegistration = new Mock<IEventRegistration<TestEvent>>();
        firstRegistration.Setup(r => r.Route(It.IsAny<Guid>(), It.IsAny<TestEvent>()))
                         .Returns([firstRoutedEvent]);

        var secondRoutedEvent = CreateRoutedEvent();
        var thirdRoutedEvent = CreateRoutedEvent();
        var secondRegistration = new Mock<IEventRegistration<TestEvent>>();
        secondRegistration.Setup(r => r.Route(It.IsAny<Guid>(), It.IsAny<TestEvent>()))
                          .Returns([secondRoutedEvent, thirdRoutedEvent]);

        var sut = CreateSut(services =>
        {
            services.AddSingleton(emptyRegistration.Object);
            services.AddSingleton(firstRegistration.Object);
            services.AddSingleton(secondRegistration.Object);
        });

        // Act
        var result = sut.Route(eventId, eventPayload);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(firstRoutedEvent, result);
        Assert.Contains(secondRoutedEvent, result);
        Assert.Contains(thirdRoutedEvent, result);

        emptyRegistration.Verify(r => r.Route(eventId, eventPayload), Times.Once);
        firstRegistration.Verify(r => r.Route(eventId, eventPayload), Times.Once);
        secondRegistration.Verify(r => r.Route(eventId, eventPayload), Times.Once);
    }

    [Fact]
    public async Task DeliverAsync_WithMatchingStrategy_CallsDeliverAsync()
    {
        // Arrange
        var routedEvent = CreateSerializedRoutedEvent();
        var cancellationToken = new CancellationToken();

        var matchingStrategyMock = new Mock<IDeliveryStrategy>();
        matchingStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(true);

        var otherStrategyMock = new Mock<IDeliveryStrategy>();
        otherStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(false);

        var anotherStrategyMock = new Mock<IDeliveryStrategy>();
        anotherStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(false);

        var sut = CreateSut(services =>
        {
            services.AddSingleton(matchingStrategyMock.Object);
            services.AddSingleton(otherStrategyMock.Object);
            services.AddSingleton(anotherStrategyMock.Object);
        });

        // Act
        await sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        matchingStrategyMock.Verify(s => s.DeliverAsync(routedEvent, cancellationToken), Times.Once);
        otherStrategyMock.Verify(s => s.DeliverAsync(routedEvent, cancellationToken), Times.Never);
        anotherStrategyMock.Verify(s => s.DeliverAsync(routedEvent, cancellationToken), Times.Never);
    }

    [Fact]
    public async Task DeliverAsync_WithNoMatchingStrategies_ThrowsInvalidOperationException()
    {
        // Arrange
        var routedEvent = CreateSerializedRoutedEvent();
        var cancellationToken = new CancellationToken();

        var notMatchingStrategyMock = new Mock<IDeliveryStrategy>();
        notMatchingStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(false);

        var sut = CreateSut(services =>
        {
            services.AddSingleton(_ => notMatchingStrategyMock.Object);
        });

        // Act
        Task Act() => sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task DeliverAsync_WithMoreThanOneMatchingStrategy_ThrowsInvalidOperationException()
    {
        // Arrange
        var routedEvent = CreateSerializedRoutedEvent();
        var cancellationToken = new CancellationToken();

        var matchingStrategyMock = new Mock<IDeliveryStrategy>();
        matchingStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(true);

        var otherStrategyMock = new Mock<IDeliveryStrategy>();
        otherStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(true);

        var sut = CreateSut(services =>
        {
            services.AddSingleton(matchingStrategyMock.Object);
            services.AddSingleton(otherStrategyMock.Object);
        });

        // Act
        Task Act() => sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    private static ScopedEventBroker CreateSut(Action<IServiceCollection> configure)
    {
        var internals = new Internals(typeof(TestModule));
        configure(internals.GetServiceCollection());
        internals.Build();
        return new ScopedEventBroker(internals);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), eventId, handlerId, Route.Create(), new TestEvent());
    }

    private static SerializedRoutedEvent CreateSerializedRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new SerializedRoutedEvent(Guid.NewGuid(), eventId, handlerId, Route.Create(), new TestEvent());
    }
}
