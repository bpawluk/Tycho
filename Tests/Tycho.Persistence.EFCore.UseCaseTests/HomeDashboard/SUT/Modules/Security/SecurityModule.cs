using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security;

[TychoDefinition]
public partial class SecurityModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<GetSecurityEventsRequest, GetSecurityEventsRequest.Response>()
              .HandlesWith<GetSecurityEventsRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<SensorEvent<MotionDetected>>()
              .HandlesWith<MotionDetectedHandler>();

        module.Expects<SensorEvent<DoorOpened>>()
              .HandlesWith<DoorOpenedHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<SecurityDbContext>();
    }

    protected override async Task Startup(IServiceProvider module, CancellationToken cancellationToken)
    {
        SecurityDbContext context = module.GetRequiredService<SecurityDbContext>();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}
