using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;

public record SetReadingRequest(string SensorId, IReading Reading, DateTimeOffset RecordedAt) : IRequest;
