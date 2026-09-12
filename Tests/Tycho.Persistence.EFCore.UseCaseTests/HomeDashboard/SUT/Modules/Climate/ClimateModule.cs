using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate;

[TychoDefinition]
public partial class ClimateModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<GetTemperatureReadingsRequest, GetTemperatureReadingsRequest.Response>()
              .HandlesWith<GetTemperatureReadingsRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<SensorEvent<TemperatureReading>>()
              .HandlesWith<SensorEventHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<ClimateDbContext>();
    }

    protected override async Task Startup(IServiceProvider module, CancellationToken cancellationToken)
    {
        ClimateDbContext context = module.GetRequiredService<ClimateDbContext>();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}
