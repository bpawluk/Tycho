using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Tycho.Events.Routing;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.UnitTests.Outbox;

public class OutboxConsumerTests
{
    private readonly OutboxConsumerSettings _settings;
    private readonly Mock<DbSet<OutboxMessage>> _dbSetMock;
    private readonly Mock<TychoDbContext> _dbContextMock;

    private readonly OutboxConsumer _sut;

    public OutboxConsumerTests()
    {
        _settings = new OutboxConsumerSettings()
        {
            MaxDeliveryCount = 3,
            ProcessingStateExpiration = TimeSpan.FromMinutes(5)
        };

        _dbSetMock = new Mock<DbSet<OutboxMessage>>();
        _dbContextMock = new Mock<TychoDbContext>();
        _dbContextMock.Setup(db => db.Set<OutboxMessage>())
                      .Returns(() => _dbSetMock.Object);

        _sut = new OutboxConsumer(_dbContextMock.Object, _settings);
    }

    [Fact]
    public async Task Read_WithNewMessages_ReadsTheMessages()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        IReadOnlyCollection<OutboxMessageState> initialMessageStates =
        [
            new(MessageState.New, DateTime.UtcNow - TimeSpan.FromMinutes(3), 0),
            new(MessageState.New, DateTime.UtcNow - TimeSpan.FromMinutes(2), 0),
            new(MessageState.New, DateTime.UtcNow - TimeSpan.FromMinutes(1), 0),
        ];
        IReadOnlyCollection<OutboxMessage> testMessages = GetMessages(initialMessageStates);

        SetupOutboxMessages(testMessages);

        // Act
        var result = await _sut.Read(testMessages.Count, cancellationToken);

        // Assert
        AssertCorrectEntries(initialMessageStates, testMessages, result);
    }

    [Fact]
    public async Task Read_WithInProcessingMessagesAfterTimeoutBelowMaxDeliveryLimit_ReadsTheMessages()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        IReadOnlyCollection<OutboxMessageState> initialMessageStates =
        [
            new(MessageState.Processing, DateTime.UtcNow - _settings.ProcessingStateExpiration * 3, 0),
            new(MessageState.Processing, DateTime.UtcNow - _settings.ProcessingStateExpiration * 2, 1),
            new(MessageState.Processing, DateTime.UtcNow - _settings.ProcessingStateExpiration * 1, 2),
        ];
        IReadOnlyCollection<OutboxMessage> testMessages = GetMessages(initialMessageStates);

        SetupOutboxMessages(testMessages);

        // Act
        var result = await _sut.Read(testMessages.Count, cancellationToken);

        // Assert
        AssertCorrectEntries(initialMessageStates, testMessages, result);
    }

    [Fact]
    public async Task Read_WithInProcessingMessagesBeforeTimeout_IgnoresTheMessages()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        IReadOnlyCollection<OutboxMessageState> initialMessageStates =
        [
            new(MessageState.Processing, DateTime.UtcNow - _settings.ProcessingStateExpiration * 0.3, 0),
            new(MessageState.Processing, DateTime.UtcNow - _settings.ProcessingStateExpiration * 0.2, 1),
            new(MessageState.Processing, DateTime.UtcNow - _settings.ProcessingStateExpiration * 0.1, 2),
        ];
        IReadOnlyCollection<OutboxMessage> testMessages = GetMessages(initialMessageStates);

        SetupOutboxMessages(testMessages);

        // Act
        var result = await _sut.Read(testMessages.Count, cancellationToken);

        // Assert
        AssertCorrectEntries([], [], result);
    }

    [Fact]
    public async Task Read_WithInProcessingMessagesAboveMaxDeliveryLimit_IgnoresTheMessages()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        IReadOnlyCollection<OutboxMessageState> initialMessageStates =
        [
            new(MessageState.Processing, DateTime.UtcNow - _settings.ProcessingStateExpiration * 3, _settings.MaxDeliveryCount),
            new(MessageState.Processing, DateTime.UtcNow - _settings.ProcessingStateExpiration * 2, _settings.MaxDeliveryCount),
            new(MessageState.Processing, DateTime.UtcNow - _settings.ProcessingStateExpiration * 1, _settings.MaxDeliveryCount),
        ];
        IReadOnlyCollection<OutboxMessage> testMessages = GetMessages(initialMessageStates);

        SetupOutboxMessages(testMessages);

        // Act
        var result = await _sut.Read(testMessages.Count, cancellationToken);

        // Assert
        AssertCorrectEntries([], [], result);
    }

    [Fact]
    public async Task Read_WithFailedMessagesBelowMaxDeliveryLimit_ReadsTheMessages()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        IReadOnlyCollection<OutboxMessageState> initialMessageStates =
        [
            new(MessageState.Failed, DateTime.UtcNow - TimeSpan.FromMinutes(3), 0),
            new(MessageState.Failed, DateTime.UtcNow - TimeSpan.FromMinutes(2), 0),
            new(MessageState.Failed, DateTime.UtcNow - TimeSpan.FromMinutes(1), 0),
        ];
        IReadOnlyCollection<OutboxMessage> testMessages = GetMessages(initialMessageStates);

        SetupOutboxMessages(testMessages);

        // Act
        var result = await _sut.Read(testMessages.Count, cancellationToken);

        // Assert
        AssertCorrectEntries(initialMessageStates, testMessages, result);
    }

    [Fact]
    public async Task Read_WithFailedMessagesAboveMaxDeliveryLimit_IgnoresTheMessages()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        IReadOnlyCollection<OutboxMessageState> initialMessageStates =
        [
            new(MessageState.Failed, DateTime.UtcNow - TimeSpan.FromMinutes(3), _settings.MaxDeliveryCount),
            new(MessageState.Failed, DateTime.UtcNow - TimeSpan.FromMinutes(2), _settings.MaxDeliveryCount),
            new(MessageState.Failed, DateTime.UtcNow - TimeSpan.FromMinutes(1), _settings.MaxDeliveryCount),
        ];
        IReadOnlyCollection<OutboxMessage> testMessages = GetMessages(initialMessageStates);

        SetupOutboxMessages(testMessages);

        // Act
        var result = await _sut.Read(testMessages.Count, cancellationToken);

        // Assert
        AssertCorrectEntries([], [], result);
    }

    [Fact]
    public async Task MarkAsProcessed_WithExistinMessage_RemovesItFromOutbox()
    {
        // Arrange
        var entry = new OutboxEntry(new("event", "handler", "module"), "{}");
        var cancellationToken = new CancellationToken();

        var message = new OutboxMessage();
        _dbSetMock.Setup(m => m.FindAsync(new object[] { entry.Id }, cancellationToken))
                  .ReturnsAsync(message);

        // Act
        await _sut.MarkAsProcessed(entry, cancellationToken);

        // Assert
        _dbSetMock.Verify(m => m.Remove(message), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task MarkAsProcessed_WithMissingMessage_IgnoresTheRequest()
    {
        // Arrange
        var entry = new OutboxEntry(new("event", "handler", "module"), "{}");
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.MarkAsProcessed(entry, cancellationToken);

        // Assert
        _dbSetMock.Verify(m => m.Remove(It.IsAny<OutboxMessage>()), Times.Never);
        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MarkAsFailed_WithExistinMessage_UpdatesItsState()
    {
        // Arrange
        var entry = new OutboxEntry(new("event", "handler", "module"), "{}");
        var cancellationToken = new CancellationToken();

        var message = new OutboxMessage();
        _dbSetMock.Setup(m => m.FindAsync(new object[] { entry.Id }, cancellationToken))
                  .ReturnsAsync(message);

        // Act
        await _sut.MarkAsFailed(entry, cancellationToken);

        // Assert
        Assert.Equal(MessageState.Failed, message.State);
        Assert.Equal(DateTime.UtcNow, message.Updated, TimeSpan.FromSeconds(1));
        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task MarkAsFailed_WithMissingMessage_IgnoresTheRequest()
    {
        // Arrange
        var entry = new OutboxEntry(new("event", "handler", "module"), "{}");
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.MarkAsProcessed(entry, cancellationToken);

        // Assert
        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static IReadOnlyCollection<OutboxMessage> GetMessages(IReadOnlyCollection<OutboxMessageState> initialMessageStates)
    {
        return initialMessageStates.Select((state, index) => new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Handler = $"event@{index}-handler@{index}-module@{index}",
            Payload = $"{{\"index\":{index}}}",
            State = state.State,
            Created = state.Updated - TimeSpan.FromMinutes(state.State is MessageState.New ? 0 : 1),
            Updated = state.Updated,
            DeliveryCount = state.DeliveryCount
        }).ToArray();
    }

    private void SetupOutboxMessages(IReadOnlyCollection<OutboxMessage> testMessages)
    {
        IReadOnlyCollection<OutboxMessage> additionalMessages =
        [
            new() { Id = Guid.NewGuid(), State = MessageState.Processing, Created = new DateTime(2020, 12, 30), Updated = DateTime.UtcNow, DeliveryCount = 0 },
            new() { Id = Guid.NewGuid(), State = MessageState.Processing, Created = new DateTime(2020, 10, 16), Updated = DateTime.UtcNow, DeliveryCount = 1 },
            new() { Id = Guid.NewGuid(), State = MessageState.Processing, Created = new DateTime(2020, 8, 8), Updated = DateTime.UtcNow, DeliveryCount = 2 },
            new() { Id = Guid.NewGuid(), State = MessageState.Processing, Created = new DateTime(2020, 6, 4), Updated = new DateTime(2020, 6, 6), DeliveryCount = _settings.MaxDeliveryCount },
            new() { Id = Guid.NewGuid(), State = MessageState.Failed, Created = new DateTime(2020, 4, 2), Updated = new DateTime(2020, 4, 4), DeliveryCount = _settings.MaxDeliveryCount },
            new() { Id = Guid.NewGuid(), State = MessageState.Failed, Created = new DateTime(2020, 2, 1), Updated = new DateTime(2020, 2, 2), DeliveryCount = _settings.MaxDeliveryCount },
        ];
        _dbContextMock.SetupSequence(x => x.Set<OutboxMessage>())
                      .ReturnsDbSet(testMessages.Concat(additionalMessages));
    }

    private static void AssertCorrectEntries(
        IReadOnlyCollection<OutboxMessageState> initialMessageStates,
        IReadOnlyCollection<OutboxMessage> readMessages,
        IReadOnlyCollection<OutboxEntry> returnedEntries)
    {
        Assert.Equal(initialMessageStates.Count, readMessages.Count);
        Assert.Equal(readMessages.Count, returnedEntries.Count);

        for (int i = 0; i < readMessages.Count; i++)
        {
            var initialState = initialMessageStates.ElementAt(i);
            var readMessage = readMessages.ElementAt(i);
            Assert.Equal(initialState.DeliveryCount + 1, readMessage.DeliveryCount);
            Assert.Equal(MessageState.Processing, readMessage.State);
            Assert.Equal(DateTime.UtcNow, readMessage.Updated, TimeSpan.FromMinutes(1));

            var returnedEntry = returnedEntries.ElementAt(i);
            Assert.Equal(readMessage.Id, returnedEntry.Id);
            Assert.Equal(HandlerIdentity.FromString(readMessage.Handler), returnedEntry.HandlerIdentity);
            Assert.Equal(readMessage.Payload, returnedEntry.Payload);
        }
    }

    private record OutboxMessageState(MessageState State, DateTime Updated, uint DeliveryCount);
}