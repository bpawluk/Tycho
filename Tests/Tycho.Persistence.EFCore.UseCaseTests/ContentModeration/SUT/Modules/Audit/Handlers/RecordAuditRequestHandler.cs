using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Handlers;

internal class RecordAuditRequestHandler(AuditDbContext dbContext, IAuditModulePublisher publisher)
    : ITransactionalRequestHandler<RecordAuditRequest>
{
    public async Task HandleAsync(RecordAuditRequest requestData, CancellationToken cancellationToken)
    {
        if (await dbContext.AuditEntries.FindAsync([requestData.Id], cancellationToken) is not null)
        {
            return;
        }

        dbContext.AuditEntries.Add(new AuditEntry(
            requestData.Id, requestData.Subject, requestData.SubjectId, requestData.Status));

        await publisher.PublishAsync(new AuditRecordedEvent(
            requestData.Id, requestData.Subject, requestData.SubjectId), cancellationToken);
    }
}
