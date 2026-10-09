using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Routing;

public sealed class ScopedEventRouterTests : IDisposable
{
    private readonly List<Internals> _hosts = [];

    [Fact]
    public async Task RouteAsync_WithNoRegistrations_ReturnsEmpty()
    {
        // Arrange
        ScopedEventRouter sut = CreateSut(_ => { });

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

        ScopedEventRouter sut = CreateSut(services =>
        {
            services.AddSingleton(emptyRegistration.Object);
            services.AddSingleton(firstRegistration.Object);
            services.AddSingleton(secondRegistration.Object);
        });

        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        // Act
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

    private ScopedEventRouter CreateSut(Action<IServiceCollection> configure)
    {
        var internals = new AppInternals(Host.CreateEmptyApplicationBuilder(default), typeof(ScopedEventRouterTests));
        _hosts.Add(internals);
        configure(internals.GetHostBuilder().Services);
        internals.Build();
        return new ScopedEventRouter(internals);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, InstanceIdentity.Parse("test-endpoint"), new TestEvent());
    }

    public void Dispose()
    {
        foreach (Internals host in _hosts)
        {
            host.Dispose();
        }
    }
}
