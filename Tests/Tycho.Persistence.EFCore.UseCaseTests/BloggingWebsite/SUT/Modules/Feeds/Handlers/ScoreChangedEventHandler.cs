using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Handlers;

internal class ScoreChangedEventHandler(FeedsDbContext dbContext) : ITransactionalEventHandler<ScoreChangedEvent>
{
    public async Task HandleAsync(EventContext<ScoreChangedEvent> context, CancellationToken cancellationToken)
    {
        Entry? entry = await dbContext.Entries.FindAsync([context.Payload.EntryId], cancellationToken);
        entry?.UpdateScore(context.Payload.NewScore);
    }
}
