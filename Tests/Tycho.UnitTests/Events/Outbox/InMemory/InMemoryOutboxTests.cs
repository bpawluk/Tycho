using Tycho.Events.Outbox;
using Tycho.Events.Outbox.InMemory;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Outbox.InMemory;

public class InMemoryOutboxTests
{
    private readonly OutboxActivity _outboxActivity;
    private readonly InMemoryOutbox _sut;

    public InMemoryOutboxTests()
    {
        _outboxActivity = new OutboxActivity();
        _sut = new InMemoryOutbox(_outboxActivity);
    }

    [Fact]
    public async Task Write_WithRoutedEvents_EnqueuesEntries()
    {
        // Arrange
        var entries = new List<RoutedEvent> { CreateRoutedEvent(), CreateRoutedEvent() };
        var cancellationToken = new CancellationToken();

        var notified = false;
        _outboxActivity.NewEntriesAdded += (_, _) => notified = true;

        // Act
        await _sut.Write(entries, cancellationToken);
        var result = await _sut.Read(entries.Count, cancellationToken);

        // Assert
        Assert.Equal(entries.Count, result.Count);
        foreach (var entry in entries)
        {
            Assert.Contains(entry, result);
        }
        Assert.True(notified);
    }

    [Fact]
    public async Task Write_WithEmptyCollection_DoesNotNotifyOutboxActivity()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        var notified = false;
        _outboxActivity.NewEntriesAdded += (_, _) => notified = true;

        // Act
        await _sut.Write([], cancellationToken);

        // Assert
        Assert.False(notified);
    }

    [Fact]
    public async Task Read_WithEntries_ReturnsRequestedCount()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var entries = new List<RoutedEvent> { CreateRoutedEvent(), CreateRoutedEvent(), CreateRoutedEvent() };
        await _sut.Write(entries, cancellationToken);

        // Act
        var result = await _sut.Read(2, cancellationToken);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Read_WithFewerEntriesThanRequested_ReturnsAvailableEntries()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        await _sut.Write([CreateRoutedEvent()], cancellationToken);

        // Act
        var result = await _sut.Read(5, cancellationToken);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task Read_WithNoEntries_ReturnsEmptyCollection()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _sut.Read(5, cancellationToken);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Read_ConsumesEntries_SubsequentReadReturnsEmpty()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        await _sut.Write([CreateRoutedEvent()], cancellationToken);

        // Act
        await _sut.Read(1, cancellationToken);
        var result = await _sut.Read(1, cancellationToken);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task MarkAsDelivered_ReturnsCompletedTask()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await _sut.MarkAsDelivered(entryId, cancellationToken);
    }

    [Fact]
    public async Task MarkAsFailed_ReturnsCompletedTask()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await _sut.MarkAsFailed(entryId, cancellationToken);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var handlerId = EventHandlerIdentity.Create<TestEventHandler, TestEvent>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), handlerId, new TestEvent());
    }
}
