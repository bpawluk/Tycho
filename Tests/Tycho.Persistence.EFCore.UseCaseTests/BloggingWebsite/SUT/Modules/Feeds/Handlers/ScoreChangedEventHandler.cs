using Tycho.Events;
using Tycho.Transactions;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Handlers;

internal class ScoreChangedEventHandler(FeedsDbContext dbContext) : ITransactionalEventHandler<ScoreChangedEvent>
{
    public async Task HandleAsync(EventContext<ScoreChangedEvent> context, CancellationToken cancellationToken)
    {
        var entry = await dbContext.Entries.FindAsync([context.Payload.EntryId], cancellationToken);
        if (entry != null)
        {
            entry.UpdateScore(context.Payload.NewScore);
        }
    }
}