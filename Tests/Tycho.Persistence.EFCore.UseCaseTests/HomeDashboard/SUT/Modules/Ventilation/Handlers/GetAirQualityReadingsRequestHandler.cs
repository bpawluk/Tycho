using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Persistence;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Handlers;

internal class GetAirQualityReadingsRequestHandler(VentilationDbContext dbContext)
    : IRequestHandler<GetAirQualityReadingsRequest, GetAirQualityReadingsRequest.Response>
{
    public async Task<GetAirQualityReadingsRequest.Response> HandleAsync(
        GetAirQualityReadingsRequest requestData,
        CancellationToken cancellationToken)
    {
        GetAirQualityReadingsRequest.AirQualityReading[] readings = await dbContext.AirQualityReadings
            .OrderBy(reading => reading.Id)
            .Select(reading => new GetAirQualityReadingsRequest.AirQualityReading(
                reading.SensorId,
                reading.Co2Ppm,
                reading.Pm25,
                reading.RecordedAt))
            .ToArrayAsync(cancellationToken);
        return new(readings);
    }
}
