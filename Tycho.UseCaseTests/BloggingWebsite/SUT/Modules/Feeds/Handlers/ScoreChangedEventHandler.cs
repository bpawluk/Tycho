using Tycho.Events;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Handlers;

internal class ScoreChangedEventHandler(IUnitOfWork unitOfWork) : IEventHandler<ScoreChangedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(ScoreChangedEvent eventData, CancellationToken cancellationToken)
    {
        var entries = _unitOfWork.Set<Entry>();
        var entry = await entries.FindAsync([eventData.EntryId], cancellationToken);
        if (entry != null)
        {
            entry.UpdateScore(eventData.NewScore);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}