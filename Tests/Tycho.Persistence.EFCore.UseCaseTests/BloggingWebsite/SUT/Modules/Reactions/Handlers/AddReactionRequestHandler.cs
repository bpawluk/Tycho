using Tycho.Transactions;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Outgoing;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Domain;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Handlers;

internal class AddReactionRequestHandler(ReactionsDbContext dbContext, ReactionsModule.IPublisher publisher) : ITransactionalRequestHandler<AddReactionRequest>
{
    public async Task HandleAsync(AddReactionRequest requestData, CancellationToken cancellationToken)
    {
        var targetProvider = new TargetProvider(dbContext);

        var reactionTarget = await targetProvider.GetTarget(requestData.TargetId, cancellationToken);
        reactionTarget.AddReaction();

        var scoreChangedEvent = new ScoreChangedEvent(reactionTarget.Id, reactionTarget.Score);
        await publisher.PublishAsync(scoreChangedEvent, cancellationToken);
    }
}