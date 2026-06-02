using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard;

internal class TestData
{
    public Readings InitialReadings { get; } =
    [
        new("living-room-climate", new TemperatureReading(21.5m), RecordedAt(minutes: 0)),
        new("bedroom-climate", new TemperatureReading(19.75m), RecordedAt(minutes: 1)),
        new("kitchen-air", new AirQualityReading(875, 12), RecordedAt(minutes: 2)),
        new("hallway-motion", new MotionDetected("Hallway"), RecordedAt(minutes: 3)),
        new("front-door", new DoorOpened("Front Door"), RecordedAt(minutes: 4)),
    ];

    public TemperatureReadings GetTemperatureReadings()
    {
        return
        [
            .. InitialReadings
                .Where(reading => reading.Reading is TemperatureReading)
                .Select(reading =>
                {
                    var temperature = (TemperatureReading)reading.Reading;
                    return new TemperatureReadingExpectation(reading.SensorId, temperature.Celsius, reading.RecordedAt);
                })
        ];
    }

    public AirQualityReadings GetAirQualityReadings()
    {
        return
        [
            .. InitialReadings
                .Where(reading => reading.Reading is AirQualityReading)
                .Select(reading =>
                {
                    var airQuality = (AirQualityReading)reading.Reading;
                    return new AirQualityReadingExpectation(
                        reading.SensorId,
                        airQuality.Co2Ppm,
                        airQuality.Pm25,
                        reading.RecordedAt);
                })
        ];
    }

    public SecurityEvents GetSecurityEvents()
    {
        return
        [
            .. InitialReadings
                .Where(reading => reading.Reading is MotionDetected or DoorOpened)
                .Select(reading => reading.Reading switch
                {
                    MotionDetected motion => new SecurityEventExpectation(
                        reading.SensorId,
                        GetSecurityEventsRequest.SecurityEventKind.MotionDetected,
                        motion.Zone,
                        reading.RecordedAt),
                    DoorOpened door => new SecurityEventExpectation(
                        reading.SensorId,
                        GetSecurityEventsRequest.SecurityEventKind.DoorOpened,
                        door.Door,
                        reading.RecordedAt),
                    _ => throw new InvalidOperationException()
                })
        ];
    }

    private static DateTimeOffset RecordedAt(int minutes)
    {
        return new DateTimeOffset(2026, 1, 1, 12, minutes, 0, TimeSpan.Zero);
    }

    public class Readings : List<SensorReading>;

    public record SensorReading(string SensorId, IReading Reading, DateTimeOffset RecordedAt);

    public class TemperatureReadings : List<TemperatureReadingExpectation>
    {
        public bool Match(GetTemperatureReadingsRequest.Response response)
        {
            return Count == response.Readings.Count &&
                   this.All(item => response.Readings.Any(item.Matches));
        }
    }

    public record TemperatureReadingExpectation(string SensorId, decimal Celsius, DateTimeOffset RecordedAt)
    {
        public bool Matches(GetTemperatureReadingsRequest.TemperatureReading fetchedReading)
        {
            return SensorId == fetchedReading.SensorId &&
                   Celsius == fetchedReading.Celsius &&
                   RecordedAt == fetchedReading.RecordedAt;
        }
    }

    public class AirQualityReadings : List<AirQualityReadingExpectation>
    {
        public bool Match(GetAirQualityReadingsRequest.Response response)
        {
            return Count == response.Readings.Count &&
                   this.All(item => response.Readings.Any(item.Matches));
        }
    }

    public record AirQualityReadingExpectation(string SensorId, int Co2Ppm, int Pm25, DateTimeOffset RecordedAt)
    {
        public bool Matches(GetAirQualityReadingsRequest.AirQualityReading fetchedReading)
        {
            return SensorId == fetchedReading.SensorId &&
                   Co2Ppm == fetchedReading.Co2Ppm &&
                   Pm25 == fetchedReading.Pm25 &&
                   RecordedAt == fetchedReading.RecordedAt;
        }
    }

    public class SecurityEvents : List<SecurityEventExpectation>
    {
        public bool Match(GetSecurityEventsRequest.Response response)
        {
            return Count == response.Events.Count &&
                   this.All(item => response.Events.Any(item.Matches));
        }
    }

    public record SecurityEventExpectation(
        string SensorId,
        GetSecurityEventsRequest.SecurityEventKind Kind,
        string Location,
        DateTimeOffset RecordedAt)
    {
        public bool Matches(GetSecurityEventsRequest.SecurityEvent fetchedEvent)
        {
            return SensorId == fetchedEvent.SensorId &&
                   Kind == fetchedEvent.Kind &&
                   Location == fetchedEvent.Location &&
                   RecordedAt == fetchedEvent.RecordedAt;
        }
    }
}
