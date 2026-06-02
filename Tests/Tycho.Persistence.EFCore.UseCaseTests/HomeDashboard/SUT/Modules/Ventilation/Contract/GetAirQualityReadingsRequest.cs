using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Contract;

public record GetAirQualityReadingsRequest : IRequest<GetAirQualityReadingsRequest.Response>
{
    public record Response(IReadOnlyList<AirQualityReading> Readings);

    public record AirQualityReading(string SensorId, int Co2Ppm, int Pm25, DateTimeOffset RecordedAt);
}
