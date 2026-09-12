namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

public record TemperatureReading(decimal Celsius) : IReading;
