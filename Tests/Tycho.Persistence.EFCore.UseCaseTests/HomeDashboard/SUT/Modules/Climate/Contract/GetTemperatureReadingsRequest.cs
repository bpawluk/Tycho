using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;

public record GetTemperatureReadingsRequest : IRequest<GetTemperatureReadingsRequest.Response>
{
    public record Response(IReadOnlyList<TemperatureReading> Readings);

    public record TemperatureReading(string SensorId, decimal Celsius, DateTimeOffset RecordedAt);
}
