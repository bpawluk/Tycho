using Moq;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Outbox.InMemory;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.TestUtils.Events;
using Tycho.TestUtils.Handlers;

namespace Tycho.UnitTests.Events.Outbox.InMemory;

public class InMemoryOutboxTests
{
    private readonly Mock<IEventSerializer> _eventSerializerMock;
    private readonly OutboxActivity _outboxActivity;
    private readonly InMemoryOutbox _sut;

    public InMemoryOutboxTests()
    {
        _eventSerializerMock = new Mock<IEventSerializer>();
        _outboxActivity = new OutboxActivity();
        _sut = new InMemoryOutbox(_eventSerializerMock.Object, _outboxActivity);
    }

    [Fact]
    public async Task Write_WithNullInputAndCancelledToken_ThrowsArgumentNullException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);

        // Act
        Task Act() => _sut.Write(null!, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>("routedEvents", Act);
    }

    [Fact]
    public async Task Write_WithCancelledToken_ThrowsWithoutEnqueueing()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        RoutedEvent routedEvent = CreateRoutedEvent();

        // Act
        Task Act() => _sut.Write([routedEvent], cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(Act);
        Assert.Null(await _sut.TryReadAsync(TestContext.Current.CancellationToken));
        _eventSerializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TryReadAsync_WithCancelledToken_ThrowsWithoutDequeuing()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        (SerializedRoutedEvent expected, RoutedEvent routed) = CreateSerializedAndRoutedEventPair();
        await _sut.Write([routed], TestContext.Current.CancellationToken);

        // Act
        Task Act() => _sut.TryReadAsync(cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(Act);
        OutboxEvent remaining = Assert.IsType<OutboxEvent>(await _sut.TryReadAsync(TestContext.Current.CancellationToken));
        Assert.Same(expected, remaining.RoutedEvent);
    }

    [Fact]
    public async Task MarkAsDeliveredAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        var entryId = Guid.NewGuid();
        var claimId = Guid.NewGuid();

        // Act
        Task Act() => _sut.MarkAsDeliveredAsync(entryId, claimId, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(Act);
    }

    [Fact]
    public async Task MarkAsFailedAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        var entryId = Guid.NewGuid();
        var claimId = Guid.NewGuid();

        // Act
        Task Act() => _sut.MarkAsFailedAsync(entryId, claimId, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(Act);
    }

    [Fact]
    public async Task Write_WhenSerializationCancels_ThrowsWithoutEnqueueingOrNotifying()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        (SerializedRoutedEvent serialized, RoutedEvent routed) = CreateSerializedAndRoutedEventPair();
        _eventSerializerMock.Setup(serializer => serializer.Serialize(routed))
            .Returns(() =>
            {
                cancellation.Cancel();
                return serialized;
            });
        bool notified = false;
        _outboxActivity.NewEntriesAdded += (_, _) => notified = true;

        // Act
        Task Act() => _sut.Write([routed], cancellation.Token);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(Act);
        Assert.Null(await _sut.TryReadAsync(TestContext.Current.CancellationToken));
        Assert.False(notified);
    }

    [Fact]
    public async Task Write_WithRoutedEvents_EnqueuesEntries()
    {
        // Arrange
        var entries = new List<(SerializedRoutedEvent Serialized, RoutedEvent Routed)>
        {
            CreateSerializedAndRoutedEventPair(),
            CreateSerializedAndRoutedEventPair(),
            CreateSerializedAndRoutedEventPair()
        };
        var cancellationToken = new CancellationToken();

        bool notified = false;
        _outboxActivity.NewEntriesAdded += (_, _) => notified = true;

        // Act
        await _sut.Write([.. entries.Select(e => e.Routed)], cancellationToken);
        var result = new List<OutboxEvent>();
        for (int i = 0; i < entries.Count; i++)
        {
            OutboxEvent? outboxEvent = await _sut.TryReadAsync(cancellationToken);
            Assert.NotNull(outboxEvent);
            result.Add(outboxEvent);
        }

        // Assert
        Assert.Equal(entries.Count, result.Count);
        SerializedRoutedEvent[] deliveredEvents = [.. result.Select(outboxEvent => outboxEvent.RoutedEvent)];
        foreach ((SerializedRoutedEvent? serialized, RoutedEvent _) in entries)
        {
            Assert.Contains(serialized, deliveredEvents);
        }
        Assert.True(notified);
    }

    [Fact]
    public async Task Write_WithEmptyCollection_DoesNotNotifyOutboxActivity()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        bool notified = false;
        _outboxActivity.NewEntriesAdded += (_, _) => notified = true;

        // Act
        await _sut.Write([], cancellationToken);

        // Assert
        Assert.False(notified);
    }

    [Fact]
    public async Task TryReadAsync_WithEntries_ReturnsOldestEntry()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var entries = new List<(SerializedRoutedEvent Serialized, RoutedEvent Routed)>
        {
            CreateSerializedAndRoutedEventPair(),
            CreateSerializedAndRoutedEventPair()
        };
        await _sut.Write([.. entries.Select(entry => entry.Routed)], cancellationToken);

        // Act
        OutboxEvent? result = await _sut.TryReadAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Same(entries[0].Serialized, result.RoutedEvent);
    }

    [Fact]
    public async Task TryReadAsync_WithNoEntries_ReturnsNull()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        OutboxEvent? result = await _sut.TryReadAsync(cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TryReadAsync_ConsumesOneEntryAtATime()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        await _sut.Write([CreateRoutedEvent(), CreateRoutedEvent()], cancellationToken);

        // Act
        OutboxEvent? firstResult = await _sut.TryReadAsync(cancellationToken);
        OutboxEvent? secondResult = await _sut.TryReadAsync(cancellationToken);
        OutboxEvent? thirdResult = await _sut.TryReadAsync(cancellationToken);

        // Assert
        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.Null(thirdResult);
    }

    [Fact]
    public async Task MarkAsDeliveredAsync_ReturnsCompletedTask()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await _sut.MarkAsDeliveredAsync(Guid.NewGuid(), claimId, cancellationToken);
    }

    [Fact]
    public async Task MarkAsFailedAsync_ReturnsCompletedTask()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await _sut.MarkAsFailedAsync(Guid.NewGuid(), claimId, cancellationToken);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, InstanceIdentity.Parse("test-endpoint"), new TestEvent());
    }

    private (SerializedRoutedEvent Serialized, RoutedEvent Routed) CreateSerializedAndRoutedEventPair()
    {
        var id = Guid.NewGuid();
        var publishId = Guid.NewGuid();
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var route = InstanceIdentity.Parse("test-endpoint");
        var serialized = new SerializedRoutedEvent(id, publishId, eventId, handlerId, route, "{}");
        var deserialized = new RoutedEvent<TestEvent>(id, publishId, eventId, handlerId, route, new TestEvent());
        _eventSerializerMock.Setup(s => s.Serialize(deserialized)).Returns(serialized);
        return (serialized, deserialized);
    }
}
