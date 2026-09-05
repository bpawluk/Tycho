using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users;

[TychoDefinition]
public partial class UsersModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<AddUserRequest, AddUserRequest.Response>()
              .HandlesWith<AddUserRequestHandler>();

        module.Expects<GetUsersRequest, GetUsersRequest.Response>()
              .HandlesWith<GetUsersRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<UserStatusChangedEvent>()
              .HandlesWith<UserStatusChangedEventHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<UsersDbContext>();
    }

    protected override async Task Startup(IServiceProvider module, CancellationToken cancellationToken)
    {
        UsersDbContext context = module.GetRequiredService<UsersDbContext>();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}
