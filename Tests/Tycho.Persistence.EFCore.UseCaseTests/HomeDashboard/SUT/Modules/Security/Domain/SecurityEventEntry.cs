using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Domain;

internal class SecurityEventEntry(
    string sensorId,
    GetSecurityEventsRequest.SecurityEventKind kind,
    string location,
    DateTimeOffset recordedAt)
{
    public int Id { get; private set; }

    public string SensorId { get; private set; } = sensorId;

    public GetSecurityEventsRequest.SecurityEventKind Kind { get; private set; } = kind;

    public string Location { get; private set; } = location;

    public DateTimeOffset RecordedAt { get; private set; } = recordedAt;
}
