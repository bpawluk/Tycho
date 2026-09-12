namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

public record AirQualityReading(int Co2Ppm, int Pm25) : IReading;
