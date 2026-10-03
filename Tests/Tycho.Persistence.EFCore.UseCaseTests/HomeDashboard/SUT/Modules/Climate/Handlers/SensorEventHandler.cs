using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Persistence;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Contract;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Handlers;

internal class SensorEventHandler(ClimateDbContext dbContext, RoomSettings settings) : ITransactionalEventHandler<SensorEvent<TemperatureReading>>
{
    public Task HandleAsync(EventContext<SensorEvent<TemperatureReading>> context, CancellationToken cancellationToken)
    {
        if (!context.Payload.SensorId.StartsWith(settings.Room, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Sensor ID '{context.Payload.SensorId}' does not match room '{settings.Room}'.");
        }

        var entry = new TemperatureReadingEntry(
            settings.Room,
            context.Payload.SensorId,
            context.Payload.Reading.Celsius,
            context.Payload.RecordedAt);
        dbContext.TemperatureReadings.Add(entry);

        return Task.CompletedTask;
    }
}
