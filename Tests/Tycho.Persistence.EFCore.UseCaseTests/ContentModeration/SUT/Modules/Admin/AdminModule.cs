using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin;

[TychoDefinition]
public partial class AdminModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<GetAuthorRequest, GetAuthorRequest.Response>();

        module.Expects<RemovePostRequest>()
              .HandlesWith<RemovePostRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<PostRemovedEvent>()
              .Exposes();

        module.Expects<UserBannedEvent>()
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<AdminDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using AdminDbContext context = app.GetRequiredService<AdminDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
