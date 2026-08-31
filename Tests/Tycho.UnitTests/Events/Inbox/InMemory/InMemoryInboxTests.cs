using Moq;
using Tycho.Events.Inbox;
using Tycho.Events.Inbox.InMemory;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Inbox.InMemory;

public class InMemoryInboxTests
{
    private readonly Mock<IEventSerializer> _eventSerializerMock;
    private readonly InboxActivity _inboxActivity;
    private readonly InMemoryInbox _sut;

    public InMemoryInboxTests()
    {
        _eventSerializerMock = new Mock<IEventSerializer>();
        _inboxActivity = new InboxActivity();
        _sut = new InMemoryInbox(_eventSerializerMock.Object, _inboxActivity);
    }

    [Fact]
    public async Task Write_WithRoutedEvent_EnqueuesEntry()
    {
        // Arrange
        (SerializedRoutedEvent? entry, RoutedEvent? deserializedEntry) = CreateSerializedAndRoutedEventPair();
        var cancelationToken = new CancellationToken();

        bool notified = false;
        _inboxActivity.NewEntriesAdded += (_, _) => notified = true;

        // Act
        await _sut.Write(entry, cancelationToken);
        InboxEvent? result = await _sut.TryReadAsync(cancelationToken);

        // Assert
        InboxEvent returnedEvent = Assert.IsType<InboxEvent>(result);
        Assert.Same(deserializedEntry, returnedEvent.RoutedEvent);
        Assert.Equal(Guid.Empty, returnedEvent.ClaimId);
        Assert.True(notified);
    }

    [Fact]
    public async Task TryReadAsync_WithEntries_ReturnsOldestEntry()
    {
        // Arrange
        var cancelationToken = new CancellationToken();
        (SerializedRoutedEvent firstEntry, RoutedEvent firstRoutedEvent) = CreateSerializedAndRoutedEventPair();
        await _sut.Write(firstEntry, cancelationToken);
        await _sut.Write(CreateSerializedRoutedEvent(), cancelationToken);

        // Act
        InboxEvent? result = await _sut.TryReadAsync(cancelationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Same(firstRoutedEvent, result.RoutedEvent);
    }

    [Fact]
    public async Task TryReadAsync_WithNoEntries_ReturnsNull()
    {
        // Arrange
        var cancelationToken = new CancellationToken();

        // Act
        InboxEvent? result = await _sut.TryReadAsync(cancelationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TryReadAsync_ConsumesOneEntryAtATime()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        await _sut.Write(CreateSerializedRoutedEvent(), cancellationToken);
        await _sut.Write(CreateSerializedRoutedEvent(), cancellationToken);

        // Act
        InboxEvent? firstResult = await _sut.TryReadAsync(cancellationToken);
        InboxEvent? secondResult = await _sut.TryReadAsync(cancellationToken);
        InboxEvent? thirdResult = await _sut.TryReadAsync(cancellationToken);

        // Assert
        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.Null(thirdResult);
    }

    [Fact]
    public async Task MarkAsHandledAsync_ReturnsCompletedTask()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var cancelationToken = new CancellationToken();

        // Act
        bool result = await _sut.MarkAsHandledAsync(claimId, cancelationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task MarkAsFailedAsync_ReturnsCompletedTask()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var cancelationToken = new CancellationToken();

        // Act
        bool result = await _sut.MarkAsFailedAsync(claimId, cancelationToken);

        // Assert
        Assert.True(result);
    }

    private static SerializedRoutedEvent CreateSerializedRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new SerializedRoutedEvent(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, Route.Create(), "{}");
    }

    private (SerializedRoutedEvent, RoutedEvent) CreateSerializedAndRoutedEventPair()
    {
        var id = Guid.NewGuid();
        var publishId = Guid.NewGuid();
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var route = Route.Create();
        var serialized = new SerializedRoutedEvent(id, publishId, eventId, handlerId, route, "{}");
        var deserialized = new RoutedEvent<TestEvent>(id, publishId, eventId, handlerId, route, new TestEvent());
        _eventSerializerMock.Setup(s => s.Deserialize(serialized)).Returns(deserialized);
        return (serialized, deserialized);
    }
}
