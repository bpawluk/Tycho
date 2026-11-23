namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

[TychoId("temperature-reading")]
public record TemperatureReading(decimal Celsius) : IReading;
