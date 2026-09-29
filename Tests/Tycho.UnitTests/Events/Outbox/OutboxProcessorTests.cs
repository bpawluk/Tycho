using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Events.Outbox;
using Tycho.Structure;
using Tycho.UnitTests._Data.Modules;
using Tycho.UnitTests._Utils;

namespace Tycho.UnitTests.Events.Outbox;

public sealed class OutboxProcessorTests
{
    [Fact]
    public async Task OutboxProcessor_WhenPollingFails_LogsProcessingFailure()
    {
        // Arrange
        var failure = new InvalidOperationException("outbox poll failed");

        var consumer = new Mock<IOutboxConsumer>();
        consumer
            .Setup(item => item.TryReadAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(failure);

        var logger = new Mock<ILogger<OutboxProcessor>>();
        logger
            .Setup(item => item.IsEnabled(LogLevel.Error))
            .Returns(true);

        using var internals = new Internals(Host.CreateEmptyApplicationBuilder(default), typeof(TestModule));
        internals.GetHostBuilder().Services.AddSingleton(consumer.Object);
        internals.Build();

        using var processor = new OutboxProcessor(internals, new OutboxActivity(), logger: logger.Object);

        // Act
        await processor.StartAsync(TestContext.Current.CancellationToken);
        try
        {
            await LogAssert.WaitForLogAsync(logger, 1402, TestContext.Current.CancellationToken);
        }
        finally
        {
            await processor.StopAsync(TestContext.Current.CancellationToken);
        }

        // Assert
        LogAssert.LoggedAtLeastOnce(logger, LogLevel.Error, 1402, "OutboxProcessingFailed", failure);
    }
}
