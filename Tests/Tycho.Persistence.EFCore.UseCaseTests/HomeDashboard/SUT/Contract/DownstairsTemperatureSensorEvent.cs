using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;

[TychoId("downstairs-temperature-sensor-event")]
public record DownstairsTemperatureSensorEvent(SensorEvent<TemperatureReading> SensorEvent) : IEvent;
