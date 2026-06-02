using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Persistence;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Handlers;

internal class GetSecurityEventsRequestHandler(SecurityDbContext dbContext)
    : IRequestHandler<GetSecurityEventsRequest, GetSecurityEventsRequest.Response>
{
    public async Task<GetSecurityEventsRequest.Response> HandleAsync(
        GetSecurityEventsRequest requestData,
        CancellationToken cancellationToken)
    {
        GetSecurityEventsRequest.SecurityEvent[] events = await dbContext.SecurityEvents
            .OrderBy(securityEvent => securityEvent.Id)
            .Select(securityEvent => new GetSecurityEventsRequest.SecurityEvent(
                securityEvent.SensorId,
                securityEvent.Kind,
                securityEvent.Location,
                securityEvent.RecordedAt))
            .ToArrayAsync(cancellationToken);
        return new(events);
    }
}
