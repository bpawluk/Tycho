using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Domain;

internal class TargetProvider(ReactionsDbContext dbContext)
{
    public async Task<Target> GetTarget(int targetId, CancellationToken cancellationToken)
    {
        var target = await dbContext.Targets.FindAsync([targetId], cancellationToken);
        if (target is null)
        {
            target = new Target(targetId);
            dbContext.Targets.Add(target);
        }
        return target;
    }
}
