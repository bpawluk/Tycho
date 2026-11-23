using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;

[TychoId("upstairs-temperature-sensor-event")]
public record UpstairsTemperatureSensorEvent(SensorEvent<TemperatureReading> SensorEvent) : IEvent;
