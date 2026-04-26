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
        var (entry, deserializedEntry) = CreateSerializedAndRoutedEventPair();
        var cancelationToken = new CancellationToken();

        var notified = false;
        _inboxActivity.NewEntriesAdded += (_, _) => notified = true;

        // Act
        await _sut.Write(entry, cancelationToken);
        var result = await _sut.Read(1, cancelationToken);

        // Assert
        var returnedEvent = Assert.Single(result);
        Assert.Same(deserializedEntry, returnedEvent);
        Assert.True(notified);
    }

    [Fact]
    public async Task Read_WithEntries_ReturnsRequestedCount()
    {
        // Arrange
        var cancelationToken = new CancellationToken();
        await _sut.Write(CreateSerializedRoutedEvent(), cancelationToken);
        await _sut.Write(CreateSerializedRoutedEvent(), cancelationToken);
        await _sut.Write(CreateSerializedRoutedEvent(), cancelationToken);

        // Act
        var result = await _sut.Read(2, cancelationToken);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Read_WithFewerEntriesThanRequested_ReturnsAvailableEntries()
    {
        // Arrange
        var cancelationToken = new CancellationToken();
        await _sut.Write(CreateSerializedRoutedEvent(), cancelationToken);

        // Act
        var result = await _sut.Read(5, cancelationToken);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task Read_WithNoEntries_ReturnsEmptyCollection()
    {
        // Arrange
        var cancelationToken = new CancellationToken();

        // Act
        var result = await _sut.Read(5, cancelationToken);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Read_ConsumesEntries_SubsequentReadReturnsEmpty()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        await _sut.Write(CreateSerializedRoutedEvent(), cancellationToken);

        // Act
        await _sut.Read(1, cancellationToken);
        var result = await _sut.Read(1, cancellationToken);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task MarkAsHandled_ReturnsCompletedTask()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var cancelationToken = new CancellationToken();

        // Act & Assert
        await _sut.MarkAsHandled(entryId, cancelationToken);
    }

    [Fact]
    public async Task MarkAsFailed_ReturnsCompletedTask()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var cancelationToken = new CancellationToken();

        // Act & Assert
        await _sut.MarkAsFailed(entryId, cancelationToken);
    }

    private static SerializedRoutedEvent CreateSerializedRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new SerializedRoutedEvent(Guid.NewGuid(), eventId, handlerId, Route.Create(), new TestEvent());
    }

    private (SerializedRoutedEvent, RoutedEvent) CreateSerializedAndRoutedEventPair()
    {
        var id = Guid.NewGuid();
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var route = Route.Create();
        var serialized = new SerializedRoutedEvent(id, eventId, handlerId, route, new TestEvent());
        var deserialized = new RoutedEvent<TestEvent>(id, eventId, handlerId, route, new TestEvent());
        _eventSerializerMock.Setup(s => s.Deserialize(serialized)).Returns(deserialized);
        return (serialized, deserialized);
    }
}
