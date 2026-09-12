using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Handlers;

internal class SensorEventHandler(ClimateDbContext dbContext) : ITransactionalEventHandler<SensorEvent<TemperatureReading>>
{
    public Task HandleAsync(EventContext<SensorEvent<TemperatureReading>> context, CancellationToken cancellationToken)
    {
        var entry = new TemperatureReadingEntry(
            context.Payload.SensorId,
            context.Payload.Reading.Celsius,
            context.Payload.RecordedAt);
        dbContext.TemperatureReadings.Add(entry);
        return Task.CompletedTask;
    }
}
