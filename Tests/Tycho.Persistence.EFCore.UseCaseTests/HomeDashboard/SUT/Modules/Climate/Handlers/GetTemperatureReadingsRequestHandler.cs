using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Persistence;
using Tycho.Requests;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Handlers;

internal class GetTemperatureReadingsRequestHandler(ClimateDbContext dbContext, RoomSettings settings)
    : IRequestHandler<GetTemperatureReadingsRequest, GetTemperatureReadingsRequest.Response>
{
    public async Task<GetTemperatureReadingsRequest.Response> HandleAsync(GetTemperatureReadingsRequest requestData, CancellationToken cancellationToken)
    {
        GetTemperatureReadingsRequest.TemperatureReading[] readings = await dbContext.TemperatureReadings
            .Where(reading => reading.Room == settings.Room)
            .OrderBy(reading => reading.Id)
            .Select(reading => new GetTemperatureReadingsRequest.TemperatureReading(
                reading.SensorId,
                reading.Celsius,
                reading.RecordedAt))
            .ToArrayAsync(cancellationToken);
        return new(readings);
    }
}
