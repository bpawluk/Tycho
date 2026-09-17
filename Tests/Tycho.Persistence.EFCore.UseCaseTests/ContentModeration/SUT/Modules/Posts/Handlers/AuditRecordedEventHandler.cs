using Tycho.Events;
using Tycho.Transactions;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class AuditRecordedEventHandler(PostsDbContext dbContext) : ITransactionalEventHandler<AuditRecordedEvent>
{
    public async Task HandleAsync(EventContext<AuditRecordedEvent> context, CancellationToken cancellationToken)
    {
        if (context.Payload.Subject != "Post")
        {
            throw new InvalidOperationException("An audit confirmation reached the wrong module.");
        }

        Post subject = await dbContext.Posts.FindAsync([context.Payload.SubjectId], cancellationToken)
                    ?? throw new ArgumentException($"There is no post with ID {context.Payload.SubjectId}");

        subject.AuditId = context.Payload.AuditId;
    }
}
