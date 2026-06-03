using Microsoft.EntityFrameworkCore;
using Moq;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.UnitTests._Data.Events;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UnitTests.Outbox;

public class OutboxWriterTests
{
    private readonly Mock<ITransaction> _transactionMock;
    private readonly Mock<IEventSerializer> _eventSerializerMock;
    private readonly Mock<TychoDbContext> _dbContextMock;
    private readonly OutboxActivity _outboxActivity;

    private readonly Mock<DbSet<OutboxEntry>> _dbSetMock;

    private int _outboxActivityNotificationCount;
    private Action? _deferredOutboxActivityNotification;

    private readonly OutboxWriter _sut;

    public OutboxWriterTests()
    {
        _transactionMock = new Mock<ITransaction>();
        _transactionMock
            .Setup(t => t.ExecuteAfterCommit(It.IsAny<Action>()))
            .Callback<Action>(action => _deferredOutboxActivityNotification = action);

        _eventSerializerMock = new Mock<IEventSerializer>();

        _dbSetMock = new Mock<DbSet<OutboxEntry>>();
        _dbSetMock.Setup(dbSet => dbSet.AddRange(It.IsAny<IEnumerable<OutboxEntry>>()))
                  .Callback<IEnumerable<OutboxEntry>>(entries =>
                  {
                      foreach (OutboxEntry _ in entries)
                      {
                      }
                  });

        _dbContextMock = new Mock<TychoDbContext>();
        _dbContextMock
            .Setup(db => db.Set<OutboxEntry>())
            .Returns(_dbSetMock.Object);
        _dbContextMock
            .Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _outboxActivity = new OutboxActivity();
        _outboxActivity.NewEntriesAdded += (_, _) => _outboxActivityNotificationCount++;

        _sut = new OutboxWriter(
            _transactionMock.Object,
            _eventSerializerMock.Object,
            _outboxActivity,
            _dbContextMock.Object);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Write_WithMultipleRoutedEvents_AddsThemToTheOutboxAndNotifiesActivity(bool isTransactionInProgress)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _transactionMock.Setup(t => t.IsInProgress).Returns(isTransactionInProgress);

        List<RoutedEvent> routedEvents =
        [
            new RoutedEvent<TestEvent>(
                Guid.NewGuid(),
                Guid.NewGuid(),
                EventIdentity.Create<TestEvent>(),
                EventHandlerIdentity.Parse("handler-1"),
                Route.Create(),
                new TestEvent()),
            new RoutedEvent<TestEvent>(
                Guid.NewGuid(),
                Guid.NewGuid(),
                EventIdentity.Create<TestEvent>(),
                EventHandlerIdentity.Parse("handler-2"),
                Route.Create(),
                new TestEvent()),
            new RoutedEvent<TestEvent>(
                Guid.NewGuid(),
                Guid.NewGuid(),
                EventIdentity.Create<TestEvent>(),
                EventHandlerIdentity.Parse("handler-3"),
                Route.Create(),
                new TestEvent()),
        ];

        _eventSerializerMock
            .Setup(s => s.Serialize(It.IsAny<RoutedEvent>()))
            .Returns<RoutedEvent>(re => new SerializedRoutedEvent(re.Id, re.PublishId, re.EventId, re.HandlerId, re.Route, "{}"));

        // Act
        await _sut.Write(routedEvents, cancellationToken);

        // Assert
        _eventSerializerMock.Verify(s => s.Serialize(It.IsAny<RoutedEvent<TestEvent>>()), Times.Exactly(routedEvents.Count));
        _dbSetMock.Verify(db => db.AddRange(It.IsAny<IEnumerable<OutboxEntry>>()), Times.Once);

        _dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), isTransactionInProgress ? Times.Never() : Times.Once());
        _transactionMock.Verify(t => t.ExecuteAfterCommit(It.IsAny<Action>()), isTransactionInProgress ? Times.Once() : Times.Never());

        if (isTransactionInProgress)
        {
            Assert.Equal(0, _outboxActivityNotificationCount);
            Assert.NotNull(_deferredOutboxActivityNotification);
            _deferredOutboxActivityNotification!();
            Assert.Equal(1, _outboxActivityNotificationCount);
        }
        else
        {
            Assert.Equal(1, _outboxActivityNotificationCount);
            Assert.Null(_deferredOutboxActivityNotification);
        }
    }
}
