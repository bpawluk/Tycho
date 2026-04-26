using Tycho.Events.Inbox;
using Tycho.Events.Inbox.InMemory;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Inbox.InMemory;

public class InMemoryInboxTests
{
    private readonly InboxActivity _inboxActivity;
    private readonly InMemoryInbox _sut;

    public InMemoryInboxTests()
    {
        _inboxActivity = new InboxActivity();
        _sut = new InMemoryInbox(_inboxActivity);
    }

    [Fact]
    public async Task Write_WithRoutedEvent_EnqueuesEntry()
    {
        // Arrange
        var entry = CreateRoutedEvent();
        var cancelationToken = new CancellationToken();

        var notified = false;
        _inboxActivity.NewEntriesAdded += (_, _) => notified = true;

        // Act
        await _sut.Write(entry, cancelationToken);
        var result = await _sut.Read(1, cancelationToken);

        // Assert
        var returnedEvent = Assert.Single(result);
        Assert.Same(entry, returnedEvent);
        Assert.True(notified);
    }

    [Fact]
    public async Task Read_WithEntries_ReturnsRequestedCount()
    {
        // Arrange
        var cancelationToken = new CancellationToken();
        await _sut.Write(CreateRoutedEvent(), cancelationToken);
        await _sut.Write(CreateRoutedEvent(), cancelationToken);
        await _sut.Write(CreateRoutedEvent(), cancelationToken);

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
        await _sut.Write(CreateRoutedEvent(), cancelationToken);

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
        await _sut.Write(CreateRoutedEvent(), cancellationToken);

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

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), handlerId, new TestEvent());
    }
}
