using Microsoft.EntityFrameworkCore;
using Moq;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.UnitTests.Outbox;

public class OutboxWriterTests
{
    private readonly Mock<DbSet<OutboxMessage>> _dbSetMock;
    private readonly Mock<TychoDbContext> _dbContextMock;

    private readonly OutboxActivity _outboxActivity;
    private int _outboxActivityNotiicationCount;

    private readonly OutboxWriter _sut;

    public OutboxWriterTests()
    {
        _dbSetMock = new Mock<DbSet<OutboxMessage>>();
        _dbContextMock = new Mock<TychoDbContext>();
        _dbContextMock.Setup(db => db.Set<OutboxMessage>())
                      .Returns(_dbSetMock.Object);

        _outboxActivity = new();
        _outboxActivity.NewEntriesAdded += (_, _) => 
        { 
            _outboxActivityNotiicationCount++; 
        };

        _sut = new(_dbContextMock.Object, _outboxActivity);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Write_WithMultipleEntries_AddsThemToTheOutboxAndNotifiesActivity(bool shouldCommit)
    {
        // Arrange
        var entries = new List<OutboxEntry>
        {
            new(new("event-1", "handler-1", "module-1"), string.Empty),
            new(new("event-2", "handler-2", "module-2"), "{}"),
            new(new("event-3", "handler-3", "module-3"), new object())
        };
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.Write(entries, shouldCommit, cancellationToken);

        // Assert
        _dbContextMock.Verify(db => db.Set<OutboxMessage>(), Times.Once);

        _dbSetMock.Verify(db => db.AddAsync(It.IsAny<OutboxMessage>(), cancellationToken), Times.Exactly(entries.Count));
        foreach (var entry in entries)
        {
            _dbSetMock.Verify(db => db.AddAsync(It.Is<OutboxMessage>(m =>
                m.Id == entry.Id &&
                m.Handler == entry.HandlerIdentity.ToString() &&
                m.Payload == (entry.Payload as string)!), cancellationToken), Times.Once);
        }

        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), shouldCommit ? Times.Once : Times.Never);
        Assert.Equal(1, _outboxActivityNotiicationCount);
    }
}