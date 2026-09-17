using Tycho.Events;
using Tycho.Transactions;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Persistence;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;

internal class AuditRecordedEventHandler(UsersDbContext dbContext) : ITransactionalEventHandler<AuditRecordedEvent>
{
    public async Task HandleAsync(EventContext<AuditRecordedEvent> context, CancellationToken cancellationToken)
    {
        if (context.Payload.Subject != "User")
        {
            throw new InvalidOperationException("An audit confirmation reached the wrong module.");
        }

        User subject = await dbContext.Users.FindAsync([context.Payload.SubjectId], cancellationToken)
                    ?? throw new ArgumentException($"There is no user with ID {context.Payload.SubjectId}");

        subject.AuditId = context.Payload.AuditId;
    }
}
