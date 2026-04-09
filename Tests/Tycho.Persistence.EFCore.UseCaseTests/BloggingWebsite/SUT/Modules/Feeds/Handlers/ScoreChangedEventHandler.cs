using Tycho.Events;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Handlers;

internal class ScoreChangedEventHandler(IUnitOfWork unitOfWork) : IEventHandler<ScoreChangedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task HandleAsync(EventContext<ScoreChangedEvent> context, CancellationToken cancellationToken)
    {
        var entries = _unitOfWork.Set<Entry>();
        var entry = await entries.FindAsync([context.Payload.EntryId], cancellationToken);
        if (entry != null)
        {
            entry.UpdateScore(context.Payload.NewScore);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}