using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users;

[TychoDefinition]
public partial class UsersModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<AddUserRequest, AddUserRequest.Response, AddUserRequestHandler>()
              .Handles<GetUsersRequest, GetUsersRequest.Response, GetUsersRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Handles<UserStatusChangedEvent, UserStatusChangedEventHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<UsersDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using UsersDbContext context = app.GetRequiredService<UsersDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
