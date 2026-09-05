using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Handlers;

internal class MotionDetectedHandler(SecurityDbContext dbContext) : ITransactionalEventHandler<SensorEvent<MotionDetected>>
{
    public Task HandleAsync(EventContext<SensorEvent<MotionDetected>> context, CancellationToken cancellationToken)
    {
        var entry = new SecurityEventEntry(
            context.Payload.SensorId,
            GetSecurityEventsRequest.SecurityEventKind.MotionDetected,
            context.Payload.Reading.Zone,
            context.Payload.RecordedAt);
        dbContext.SecurityEvents.Add(entry);
        return Task.CompletedTask;
    }
}
