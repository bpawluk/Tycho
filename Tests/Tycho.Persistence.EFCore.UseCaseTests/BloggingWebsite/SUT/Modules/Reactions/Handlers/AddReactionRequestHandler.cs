using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Handlers;

internal class AddReactionRequestHandler(ReactionsDbContext dbContext, IReactionsModulePublisher publisher) : ITransactionalRequestHandler<AddReactionRequest>
{
    public async Task HandleAsync(AddReactionRequest requestData, CancellationToken cancellationToken)
    {
        var targetProvider = new TargetProvider(dbContext);

        Target reactionTarget = await targetProvider.GetTarget(requestData.TargetId, cancellationToken);
        reactionTarget.AddReaction();

        var scoreChangedEvent = new ScoreChangedEvent(reactionTarget.Id, reactionTarget.Score);
        await publisher.PublishAsync(scoreChangedEvent, cancellationToken);
    }
}
