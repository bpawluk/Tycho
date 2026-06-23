using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT;

[TychoDefinition]
public partial class HomeDashboardApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<SetReadingRequest>()
           .HandlesWith<SetReadingRequestHandler>();

        app.Expects<GetTemperatureReadingsRequest, GetTemperatureReadingsRequest.Response>()
           .ForwardsTo<ClimateModule>();

        app.Expects<GetAirQualityReadingsRequest, GetAirQualityReadingsRequest.Response>()
           .ForwardsTo<VentilationModule>();

        app.Expects<GetSecurityEventsRequest, GetSecurityEventsRequest.Response>()
           .ForwardsTo<SecurityModule>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Expects<SensorEvent<TemperatureReading>>()
           .ForwardsTo<ClimateModule>();

        app.Expects<SensorEvent<AirQualityReading>>()
           .ForwardsTo<VentilationModule>();

        app.Expects<SensorEvent<MotionDetected>>()
           .ForwardsTo<SecurityModule>();

        app.Expects<SensorEvent<DoorOpened>>()
           .ForwardsTo<SecurityModule>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<ClimateModule>()
           .Uses<VentilationModule>()
           .Uses<SecurityModule>();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddTychoPersistence<HomeDashboardDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using HomeDashboardDbContext context = app.GetRequiredService<HomeDashboardDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
