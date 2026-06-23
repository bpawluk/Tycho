using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
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
        module.Handles<AddReactionRequest, AddReactionRequestHandler>();
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

    protected override async Task Startup(IServiceProvider app)
    {
        using ReactionsDbContext context = app.GetRequiredService<ReactionsDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
