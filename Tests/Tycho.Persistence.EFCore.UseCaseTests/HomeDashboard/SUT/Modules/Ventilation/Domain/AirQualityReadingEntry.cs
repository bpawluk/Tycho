namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Domain;

internal class AirQualityReadingEntry(string sensorId, int co2Ppm, int pm25, DateTimeOffset recordedAt)
{
    public int Id { get; private set; }

    public string SensorId { get; private set; } = sensorId;

    public int Co2Ppm { get; private set; } = co2Ppm;

    public int Pm25 { get; private set; } = pm25;

    public DateTimeOffset RecordedAt { get; private set; } = recordedAt;
}
