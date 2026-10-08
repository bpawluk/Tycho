using Moq;
using Tycho.Events.Inbox;
using Tycho.Events.Inbox.InMemory;
using Tycho.Events.Model;
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
    public async Task Write_WithEvent_EnqueuesEntry()
    {
        // Arrange
        (SerializedEvent? entry, Event? deserializedEntry) = CreateSerializedAndEventPair();
        var cancelationToken = new CancellationToken();

        bool notified = false;
        _inboxActivity.NewEntriesAdded += (_, _) => notified = true;

        // Act
        await _sut.Write(entry, cancelationToken);
        InboxEvent? result = await _sut.TryReadAsync(cancelationToken);

        // Assert
        InboxEvent returnedEvent = Assert.IsType<InboxEvent>(result);
        Assert.Same(deserializedEntry, returnedEvent.Event);
        Assert.Equal(Guid.Empty, returnedEvent.ClaimId);
        Assert.True(notified);
    }

    [Fact]
    public async Task TryReadAsync_WithEntries_ReturnsOldestEntry()
    {
        // Arrange
        var cancelationToken = new CancellationToken();
        (SerializedEvent firstEntry, Event firstEvent) = CreateSerializedAndEventPair();
        await _sut.Write(firstEntry, cancelationToken);
        await _sut.Write(CreateSerializedEvent(), cancelationToken);

        // Act
        InboxEvent? result = await _sut.TryReadAsync(cancelationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Same(firstEvent, result.Event);
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
        await _sut.Write(CreateSerializedEvent(), cancellationToken);
        await _sut.Write(CreateSerializedEvent(), cancellationToken);

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

    private static SerializedEvent CreateSerializedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new SerializedEvent(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, "{}");
    }

    private (SerializedEvent, Event) CreateSerializedAndEventPair()
    {
        var id = Guid.NewGuid();
        var publishId = Guid.NewGuid();
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var serialized = new SerializedEvent(id, publishId, eventId, handlerId, "{}");
        var deserialized = new Event<TestEvent>(id, publishId, eventId, handlerId, new TestEvent());
        _eventSerializerMock.Setup(s => s.Deserialize(serialized)).Returns(deserialized);
        return (serialized, deserialized);
    }
}
