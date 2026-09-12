using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Persistence;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Handlers;

internal class GetTemperatureReadingsRequestHandler(ClimateDbContext dbContext)
    : IRequestHandler<GetTemperatureReadingsRequest, GetTemperatureReadingsRequest.Response>
{
    public async Task<GetTemperatureReadingsRequest.Response> HandleAsync(
        GetTemperatureReadingsRequest requestData,
        CancellationToken cancellationToken)
    {
        GetTemperatureReadingsRequest.TemperatureReading[] readings = await dbContext.TemperatureReadings
            .OrderBy(reading => reading.Id)
            .Select(reading => new GetTemperatureReadingsRequest.TemperatureReading(
                reading.SensorId,
                reading.Celsius,
                reading.RecordedAt))
            .ToArrayAsync(cancellationToken);
        return new(readings);
    }
}
