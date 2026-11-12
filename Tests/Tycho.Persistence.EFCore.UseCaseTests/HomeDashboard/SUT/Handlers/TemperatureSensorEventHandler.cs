using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Handlers;

[TychoId("route-temperature-reading")]
internal class TemperatureSensorEventHandler(IHomeDashboardAppPublisher publisher)
    : ITransactionalEventHandler<SensorEvent<TemperatureReading>>
{
    public Task HandleAsync(EventContext<SensorEvent<TemperatureReading>> context, CancellationToken cancellationToken)
    {
        // Stage forwarding in this handler's HomeDashboard outbox so it commits
        // with inbox completion. Calling a room's request endpoint here publishes
        // independently and would publish again if this transaction is retried.
        if (context.Payload.SensorId.StartsWith("downstairs-", StringComparison.Ordinal))
        {
            return publisher.PublishAsync(new DownstairsTemperatureSensorEvent(context.Payload), cancellationToken);
        }

        if (context.Payload.SensorId.StartsWith("upstairs-", StringComparison.Ordinal))
        {
            return publisher.PublishAsync(new UpstairsTemperatureSensorEvent(context.Payload), cancellationToken);
        }

        throw new ArgumentException($"Unknown room for sensor '{context.Payload.SensorId}'.", nameof(context));
    }
}
