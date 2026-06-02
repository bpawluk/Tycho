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
        module.Handles<GetTemperatureReadingsRequest, GetTemperatureReadingsRequest.Response, GetTemperatureReadingsRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Handles<SensorEvent<TemperatureReading>, SensorEventHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<ClimateDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using ClimateDbContext context = app.GetRequiredService<ClimateDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
