using Tycho.Requests;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Contract;

public record RegisterTemperatureSensorEventRequest(SensorEvent<TemperatureReading> SensorEvent) : IRequest;
