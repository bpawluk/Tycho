using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Transactions;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Handlers;

internal class TemperatureSensorEventHandler(IDownstairsModule downstairs, IUpstairsModule upstairs)
    : ITransactionalEventHandler<SensorEvent<TemperatureReading>>
{
    public Task HandleAsync(EventContext<SensorEvent<TemperatureReading>> context, CancellationToken cancellationToken)
    {
        var request = new RegisterTemperatureSensorEventRequest(context.Payload);

        if (context.Payload.SensorId.StartsWith("downstairs-", StringComparison.Ordinal))
        {
            return downstairs.ExecuteAsync(request, cancellationToken);
        }

        if (context.Payload.SensorId.StartsWith("upstairs-", StringComparison.Ordinal))
        {
            return upstairs.ExecuteAsync(request, cancellationToken);
        }

        throw new ArgumentException($"Unknown room for sensor '{context.Payload.SensorId}'.", nameof(context));
    }
}
