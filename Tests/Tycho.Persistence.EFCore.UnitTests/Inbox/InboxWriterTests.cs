using Microsoft.EntityFrameworkCore;
using Moq;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.UnitTests._Data.Events;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UnitTests.Inbox;

public class InboxWriterTests
{
    private readonly Mock<ITransaction> _transactionMock;
    private readonly Mock<TychoDbContext> _dbContextMock;
    private readonly Mock<DbSet<InboxEntry>> _dbSetMock;
    private readonly InboxActivity _inboxActivity;

    private int _inboxActivityNotificationCount;
    private Action? _deferredInboxActivityNotification;

    private readonly InboxWriter _sut;

    public InboxWriterTests()
    {
        _transactionMock = new Mock<ITransaction>();
        _transactionMock
            .Setup(t => t.ExecuteAfterCommit(It.IsAny<Action>()))
            .Callback<Action>(action => _deferredInboxActivityNotification = action);

        _dbSetMock = new Mock<DbSet<InboxEntry>>();

        _dbContextMock = new Mock<TychoDbContext>();
        _dbContextMock
            .Setup(db => db.Set<InboxEntry>())
            .Returns(_dbSetMock.Object);
        _dbContextMock
            .Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _inboxActivity = new InboxActivity();
        _inboxActivity.NewEntriesAdded += (_, _) => _inboxActivityNotificationCount++;

        _sut = new InboxWriter(
            _transactionMock.Object,
            _inboxActivity,
            _dbContextMock.Object);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Write_AddsEntryAndNotifiesActivityAfterPersistence(bool isTransactionInProgress)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _transactionMock.Setup(t => t.IsInProgress).Returns(isTransactionInProgress);
        SerializedRoutedEvent serializedEvent = CreateSerializedEvent();

        // Act
        await _sut.Write(serializedEvent, cancellationToken);

        // Assert
        _dbSetMock.Verify(
            db => db.Add(It.Is<InboxEntry>(entry =>
                entry.Id == serializedEvent.Id &&
                entry.PublishId == serializedEvent.PublishId &&
                entry.Event == serializedEvent.EventId.ToString() &&
                entry.Handler == serializedEvent.HandlerId.ToString() &&
                entry.Payload == serializedEvent.Payload)),
            Times.Once);

        _dbContextMock.Verify(
            db => db.SaveChangesAsync(cancellationToken),
            isTransactionInProgress ? Times.Never() : Times.Once());

        _transactionMock.Verify(
            t => t.ExecuteAfterCommit(It.IsAny<Action>()),
            isTransactionInProgress ? Times.Once() : Times.Never());

        if (isTransactionInProgress)
        {
            Assert.Equal(0, _inboxActivityNotificationCount);
            Assert.NotNull(_deferredInboxActivityNotification);

            _deferredInboxActivityNotification!();

            Assert.Equal(1, _inboxActivityNotificationCount);
        }
        else
        {
            Assert.Equal(1, _inboxActivityNotificationCount);
            Assert.Null(_deferredInboxActivityNotification);
        }
    }

    [Fact]
    public async Task Write_WhenPersistenceFails_DoesNotNotifyActivity()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _transactionMock.Setup(t => t.IsInProgress).Returns(false);
        _dbContextMock
            .Setup(db => db.SaveChangesAsync(cancellationToken))
            .ThrowsAsync(new InvalidOperationException("persistence failure"));

        // Act
        Task act() => _sut.Write(CreateSerializedEvent(), cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal(0, _inboxActivityNotificationCount);
        _transactionMock.Verify(t => t.ExecuteAfterCommit(It.IsAny<Action>()), Times.Never);
    }

    private static SerializedRoutedEvent CreateSerializedEvent()
    {
        return new SerializedRoutedEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            EventIdentity.Create<TestEvent>(),
            EventHandlerIdentity.Parse("test-handler"),
            Route.Empty(),
            "{}");
    }
}
