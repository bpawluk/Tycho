using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Outgoing;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Handlers;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Persistence;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions;

public class ReactionsModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<AddReactionRequest, AddReactionRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Routes<ScoreChangedEvent>()
              .Exposes();
    }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<ReactionsDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<ReactionsDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}