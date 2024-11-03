using Microsoft.EntityFrameworkCore;
using Moq;
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
        _settings = new OutboxConsumerSettings();

        _dbSetMock = new Mock<DbSet<OutboxMessage>>();
        _dbContextMock = new Mock<TychoDbContext>();
        _dbContextMock.Setup(db => db.Set<OutboxMessage>())
                      .Returns(_dbSetMock.Object);

        _sut = new OutboxConsumer(_settings, _dbContextMock.Object);
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
}