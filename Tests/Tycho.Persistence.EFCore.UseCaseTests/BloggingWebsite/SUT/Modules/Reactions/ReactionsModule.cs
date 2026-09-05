using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions;

[TychoDefinition]
public partial class ReactionsModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<AddReactionRequest>()
              .HandlesWith<AddReactionRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<ScoreChangedEvent>()
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<ReactionsDbContext>();
    }

    protected override async Task Startup(IServiceProvider module, CancellationToken cancellationToken)
    {
        ReactionsDbContext context = module.GetRequiredService<ReactionsDbContext>();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}
