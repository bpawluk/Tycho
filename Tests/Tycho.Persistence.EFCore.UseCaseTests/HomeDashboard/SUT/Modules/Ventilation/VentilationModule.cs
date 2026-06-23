using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation;

[TychoDefinition]
public partial class VentilationModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GetAirQualityReadingsRequest, GetAirQualityReadingsRequest.Response, GetAirQualityReadingsRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<SensorEvent<AirQualityReading>>()
              .HandlesWith<SensorEventHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<VentilationDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using VentilationDbContext context = app.GetRequiredService<VentilationDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
