using Moq;
using Tycho.Events;
using Tycho.Events.Publishing;
using Tycho.Events.Routing;
using Tycho.Persistence;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Publishing;

public class EventPublisherTests
{
    private readonly Mock<IOutbox> _outboxMock;

    private readonly HandlerIdentity[] _testIdentities =
    [
        new(typeof(TestEvent), typeof(TestEventHandler), typeof(TestModule)),
        new(typeof(TestEvent), typeof(TestEventOtherHandler), typeof(TestModule))
    ];

    private readonly EventPublisher _sut;

    public EventPublisherTests()
    {
        var routerMock = new Mock<IEventRouter>();
        routerMock.Setup(r => r.IdentifyHandlers<TestEvent>())
                  .Returns(_testIdentities);
        routerMock.Setup(r => r.IdentifyHandlers<OtherEvent>())
                  .Returns([]);

        _outboxMock = new Mock<IOutbox>();

        var serializerMock = new Mock<IPayloadSerializer>();
        serializerMock.Setup(s => s.Serialize(It.IsAny<IEvent>()))
                      .Returns((IEvent eventData) => SerializeMock(eventData));

        _sut = new EventPublisher(routerMock.Object, _outboxMock.Object, serializerMock.Object);
    }

    [Fact]
    public async Task Publish_ForEventWithRegisteredHandlers_AddsOutboxEntries()
    {
        // Arrange
        var eventData = new TestEvent();
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Publish(eventData, cancellationToken);

        // Assert
        _outboxMock.Verify(
            o => o.Add(
                It.Is<OutboxEntry[]>(entries =>
                    entries.Length == _testIdentities.Length &&
                    entries[0].HandlerIdentity == _testIdentities[0] &&
                    entries[1].HandlerIdentity == _testIdentities[1] &&
                    entries.All(entry => entry.Payload as string == SerializeMock(eventData))),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Publish_ForEventWithNoHandlers_DoesNotAddAnyEntries()
    {
        // Arrange
        var eventData = new OtherEvent();
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Publish(eventData, cancellationToken);

        // Assert
        _outboxMock.Verify(
            o => o.Add(
                It.IsAny<IReadOnlyCollection<OutboxEntry>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Publish_WithNullEventData_ThrowsArgumentNullException()
    {
        // Arrange
        IEvent eventData = null!;
        var cancellationToken = CancellationToken.None;

        // Act
        async Task Act()
        {
            await _sut.Publish(eventData, cancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }

    private static string SerializeMock(IEvent eventData)
    {
        return eventData.GetType().Name;
    }
}