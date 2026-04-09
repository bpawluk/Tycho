using Tycho.Persistence.EFCore;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Domain;

internal class TargetProvider(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Target> GetTarget(int targetId, CancellationToken cancellationToken)
    {
        var targets = _unitOfWork.Set<Target>();

        var target = await targets.FindAsync([targetId], cancellationToken);
        if (target is null)
        {
            target = new Target(targetId);
            targets.Add(target);
        }

        return target;
    }
}