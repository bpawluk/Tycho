using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Contract;

public record RegisterTemperatureSensorEventRequest(SensorEvent<TemperatureReading> SensorEvent) : IRequest;
