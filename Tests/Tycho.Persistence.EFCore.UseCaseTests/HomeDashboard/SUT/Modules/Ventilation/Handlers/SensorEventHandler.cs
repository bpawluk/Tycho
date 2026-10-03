using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Handlers;

internal class SensorEventHandler(VentilationDbContext dbContext) : ITransactionalEventHandler<SensorEvent<AirQualityReading>>
{
    public Task HandleAsync(EventContext<SensorEvent<AirQualityReading>> context, CancellationToken cancellationToken)
    {
        var entry = new AirQualityReadingEntry(
            context.Payload.SensorId,
            context.Payload.Reading.Co2Ppm,
            context.Payload.Reading.Pm25,
            context.Payload.RecordedAt);
        dbContext.AirQualityReadings.Add(entry);
        return Task.CompletedTask;
    }
}
