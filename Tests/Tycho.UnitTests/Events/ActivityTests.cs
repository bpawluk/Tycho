using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Events.Inbox;
using Tycho.Events.Outbox;
using Tycho.UnitTests._Utils;

namespace Tycho.UnitTests.Events;

public class ActivityTests
{
    [Fact]
    public void InboxActivity_WhenSubscriberThrows_LogsFailureAndNotifiesRemainingSubscribers()
    {
        // Arrange
        var exception = new InvalidOperationException("notification failure");
        var loggerMock = new Mock<ILogger<InboxActivity>>();
        loggerMock.Setup(logger => logger.IsEnabled(LogLevel.Error)).Returns(true);
        var sut = new InboxActivity(loggerMock.Object);
        int notificationCount = 0;
        sut.NewEntriesAdded += (_, _) => throw exception;
        sut.NewEntriesAdded += (_, _) => notificationCount++;

        // Act
        sut.NotifyNewEntriesAdded();

        // Assert
        Assert.Equal(1, notificationCount);
        LogAssert.Logged(loggerMock, LogLevel.Error, 1301, "InboxNotificationFailed", exception);
    }

    [Fact]
    public void InboxActivity_WhenFailureLoggingThrows_StillNotifiesRemainingSubscribers()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<InboxActivity>>();
        loggerMock.Setup(logger => logger.IsEnabled(LogLevel.Error))
            .Throws(new InvalidOperationException("logging failure"));
        var sut = new InboxActivity(loggerMock.Object);
        int notificationCount = 0;
        sut.NewEntriesAdded += (_, _) => throw new InvalidOperationException("subscriber failure");
        sut.NewEntriesAdded += (_, _) => notificationCount++;

        // Act
        sut.NotifyNewEntriesAdded();

        // Assert
        Assert.Equal(1, notificationCount);
        loggerMock.Verify(logger => logger.IsEnabled(LogLevel.Error), Times.Once);
    }

    [Fact]
    public void OutboxActivity_WhenSubscriberThrows_LogsFailureAndNotifiesRemainingSubscribers()
    {
        // Arrange
        var exception = new InvalidOperationException("notification failure");
        var loggerMock = new Mock<ILogger<OutboxActivity>>();
        loggerMock.Setup(logger => logger.IsEnabled(LogLevel.Error)).Returns(true);
        var sut = new OutboxActivity(loggerMock.Object);
        int notificationCount = 0;
        sut.NewEntriesAdded += (_, _) => throw exception;
        sut.NewEntriesAdded += (_, _) => notificationCount++;

        // Act
        sut.NotifyNewEntriesAdded();

        // Assert
        Assert.Equal(1, notificationCount);
        LogAssert.Logged(loggerMock, LogLevel.Error, 1401, "OutboxNotificationFailed", exception);
    }

    [Fact]
    public void OutboxActivity_WhenFailureLoggingThrows_StillNotifiesRemainingSubscribers()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<OutboxActivity>>();
        loggerMock.Setup(logger => logger.IsEnabled(LogLevel.Error))
            .Throws(new InvalidOperationException("logging failure"));
        var sut = new OutboxActivity(loggerMock.Object);
        int notificationCount = 0;
        sut.NewEntriesAdded += (_, _) => throw new InvalidOperationException("subscriber failure");
        sut.NewEntriesAdded += (_, _) => notificationCount++;

        // Act
        sut.NotifyNewEntriesAdded();

        // Assert
        Assert.Equal(1, notificationCount);
        loggerMock.Verify(logger => logger.IsEnabled(LogLevel.Error), Times.Once);
    }
}
