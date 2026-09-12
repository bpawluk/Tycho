using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Events.Inbox;
using Tycho.Events.Outbox;

namespace Tycho.UnitTests.Events;

public class ActivityTests
{
    [Fact]
    public void InboxActivity_WhenSubscriberThrows_LogsFailureAndNotifiesRemainingSubscribers()
    {
        // Arrange
        var exception = new InvalidOperationException("notification failure");
        var loggerMock = new Mock<ILogger<InboxActivity>>();
        var sut = new InboxActivity(loggerMock.Object);
        int notificationCount = 0;
        sut.NewEntriesAdded += (_, _) => throw exception;
        sut.NewEntriesAdded += (_, _) => notificationCount++;

        // Act
        sut.NotifyNewEntriesAdded();

        // Assert
        Assert.Equal(1, notificationCount);
        VerifyErrorLogged(loggerMock, exception);
    }

    [Fact]
    public void OutboxActivity_WhenSubscriberThrows_LogsFailureAndNotifiesRemainingSubscribers()
    {
        // Arrange
        var exception = new InvalidOperationException("notification failure");
        var loggerMock = new Mock<ILogger<OutboxActivity>>();
        var sut = new OutboxActivity(loggerMock.Object);
        int notificationCount = 0;
        sut.NewEntriesAdded += (_, _) => throw exception;
        sut.NewEntriesAdded += (_, _) => notificationCount++;

        // Act
        sut.NotifyNewEntriesAdded();

        // Assert
        Assert.Equal(1, notificationCount);
        VerifyErrorLogged(loggerMock, exception);
    }

    private static void VerifyErrorLogged<T>(Mock<ILogger<T>> loggerMock, Exception exception)
    {
        loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
