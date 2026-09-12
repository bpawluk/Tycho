using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    public async Task RouteAsync_WithNoRegistrations_ReturnsEmpty()
    {
        // Arrange
        ScopedEventBroker sut = CreateSut(_ => { });

        // Act
        IReadOnlyCollection<RoutedEvent> result = await sut.RouteAsync(
            Guid.NewGuid(),
            new TestEvent(),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task RouteAsync_WithMultipleRegistrations_ReturnsAllRoutedEvents()
    {
        // Arrange
        var publishId = Guid.NewGuid();
        var eventPayload = new TestEvent();

        var emptyRegistration = new Mock<IEventRegistration<TestEvent>>();
        emptyRegistration.Setup(r => r.RouteAsync(It.IsAny<Guid>(), It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync([]);

        RoutedEvent<TestEvent> firstRoutedEvent = CreateRoutedEvent();
        var firstRegistration = new Mock<IEventRegistration<TestEvent>>();
        firstRegistration.Setup(r => r.RouteAsync(It.IsAny<Guid>(), It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync([firstRoutedEvent]);

        RoutedEvent<TestEvent> secondRoutedEvent = CreateRoutedEvent();
        RoutedEvent<TestEvent> thirdRoutedEvent = CreateRoutedEvent();
        var secondRegistration = new Mock<IEventRegistration<TestEvent>>();
        secondRegistration.Setup(r => r.RouteAsync(It.IsAny<Guid>(), It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync([secondRoutedEvent, thirdRoutedEvent]);

        ScopedEventBroker sut = CreateSut(services =>
        {
            services.AddSingleton(emptyRegistration.Object);
            services.AddSingleton(firstRegistration.Object);
            services.AddSingleton(secondRegistration.Object);
        });

        // Act
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        IReadOnlyCollection<RoutedEvent> result = await sut.RouteAsync(publishId, eventPayload, cancellationToken);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(firstRoutedEvent, result);
        Assert.Contains(secondRoutedEvent, result);
        Assert.Contains(thirdRoutedEvent, result);

        emptyRegistration.Verify(r => r.RouteAsync(publishId, eventPayload, cancellationToken), Times.Once);
        firstRegistration.Verify(r => r.RouteAsync(publishId, eventPayload, cancellationToken), Times.Once);
        secondRegistration.Verify(r => r.RouteAsync(publishId, eventPayload, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeliverAsync_WithMatchingStrategy_CallsDeliverAsync()
    {
        // Arrange
        SerializedRoutedEvent routedEvent = CreateSerializedRoutedEvent();
        var cancellationToken = new CancellationToken();

        var matchingStrategyMock = new Mock<IDeliveryStrategy>();
        matchingStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(true);

        var otherStrategyMock = new Mock<IDeliveryStrategy>();
        otherStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(false);

        var anotherStrategyMock = new Mock<IDeliveryStrategy>();
        anotherStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(false);

        ScopedEventBroker sut = CreateSut(services =>
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
        SerializedRoutedEvent routedEvent = CreateSerializedRoutedEvent();
        var cancellationToken = new CancellationToken();

        var notMatchingStrategyMock = new Mock<IDeliveryStrategy>();
        notMatchingStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(false);

        ScopedEventBroker sut = CreateSut(services =>
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
        SerializedRoutedEvent routedEvent = CreateSerializedRoutedEvent();
        var cancellationToken = new CancellationToken();

        var matchingStrategyMock = new Mock<IDeliveryStrategy>();
        matchingStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(true);

        var otherStrategyMock = new Mock<IDeliveryStrategy>();
        otherStrategyMock.Setup(s => s.CanDeliver(routedEvent)).Returns(true);

        ScopedEventBroker sut = CreateSut(services =>
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
        var internals = new Internals(typeof(TestModule), Host.CreateEmptyApplicationBuilder(default));
        configure(internals.GetHostBuilder().Services);
        internals.Build();
        return new ScopedEventBroker(internals);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, Route.Create(), new TestEvent());
    }

    private static SerializedRoutedEvent CreateSerializedRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new SerializedRoutedEvent(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, Route.Create(), "{}");
    }
}
