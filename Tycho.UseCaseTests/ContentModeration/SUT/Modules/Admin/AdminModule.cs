using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Handlers;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Persistence;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin;

public class AdminModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<RemovePostRequest, RemovePostRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Routes<PostRemovedEvent>()
              .Exposes();

        module.Routes<UserBannedEvent>()
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<AdminDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<AdminDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}