using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Contract;

public record GetSecurityEventsRequest : IRequest<GetSecurityEventsRequest.Response>
{
    public record Response(IReadOnlyList<SecurityEvent> Events);

    public record SecurityEvent(string SensorId, SecurityEventKind Kind, string Location, DateTimeOffset RecordedAt);

    public enum SecurityEventKind
    {
        MotionDetected,
        DoorOpened
    }
}
