namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Domain;

internal class TemperatureReadingEntry(string sensorId, decimal celsius, DateTimeOffset recordedAt)
{
    public int Id { get; private set; }

    public string SensorId { get; private set; } = sensorId;

    public decimal Celsius { get; private set; } = celsius;

    public DateTimeOffset RecordedAt { get; private set; } = recordedAt;
}
