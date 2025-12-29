using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Outgoing;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Domain;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Handlers;

internal class AddReactionRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddReactionRequest>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(AddReactionRequest requestData, CancellationToken cancellationToken)
    {
        var targetProvider = new TargetProvider(_unitOfWork);

        var reactionTarget = await targetProvider.GetTarget(requestData.TargetId, cancellationToken);
        reactionTarget.AddReaction();

        var scoreChangedEvent = new ScoreChangedEvent(reactionTarget.Id, reactionTarget.Score);
        await _unitOfWork.Publish(scoreChangedEvent, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);
    }
}