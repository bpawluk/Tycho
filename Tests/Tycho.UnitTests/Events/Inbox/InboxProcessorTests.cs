using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Events.Inbox;
using Tycho.Structure;
using Tycho.UnitTests._Data.Modules;
using Tycho.UnitTests._Utils;

namespace Tycho.UnitTests.Events.Inbox;

public sealed class InboxProcessorTests
{
    [Fact]
    public async Task InboxProcessor_WhenPollingFails_LogsProcessingFailure()
    {
        // Arrange
        var failure = new InvalidOperationException("inbox poll failed");

        var consumer = new Mock<IInboxConsumer>();
        consumer
            .Setup(item => item.TryReadAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(failure);

        var logger = new Mock<ILogger<InboxProcessor>>();
        logger
            .Setup(item => item.IsEnabled(LogLevel.Error))
            .Returns(true);

        using var internals = new Internals(Host.CreateEmptyApplicationBuilder(default), typeof(TestModule));
        internals.GetHostBuilder().Services.AddSingleton(consumer.Object);
        internals.Build();

        using var processor = new InboxProcessor(internals, new InboxActivity(), new InboxSettings(), logger: logger.Object);

        // Act
        await processor.StartAsync(TestContext.Current.CancellationToken);
        try
        {
            await LogAssert.WaitForLogAsync(logger, 1302, TestContext.Current.CancellationToken);
        }
        finally
        {
            await processor.StopAsync(TestContext.Current.CancellationToken);
        }

        // Assert
        LogAssert.LoggedAtLeastOnce(logger, LogLevel.Error, 1302, "InboxProcessingFailed", failure);
    }
}
