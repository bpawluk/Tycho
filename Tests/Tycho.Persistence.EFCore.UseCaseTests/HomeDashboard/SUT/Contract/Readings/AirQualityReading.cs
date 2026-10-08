namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

[TychoId("air-quality-reading")]
public record AirQualityReading(int Co2Ppm, int Pm25) : IReading;
