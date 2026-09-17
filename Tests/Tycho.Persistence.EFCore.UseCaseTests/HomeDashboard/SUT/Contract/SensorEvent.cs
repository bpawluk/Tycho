using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;

public record SensorEvent<TReading>(string SensorId, TReading Reading, DateTimeOffset RecordedAt) : IEvent
    where TReading : IReading;
