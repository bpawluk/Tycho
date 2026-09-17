using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Persistence;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms;
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

        app.Expects<GetRoomTemperatureReadingsRequest, GetTemperatureReadingsRequest.Response>()
           .HandlesWith<GetRoomTemperatureReadingsRequestHandler>();

        app.Expects<GetAirQualityReadingsRequest, GetAirQualityReadingsRequest.Response>()
           .ForwardsTo<VentilationModule>();

        app.Expects<GetSecurityEventsRequest, GetSecurityEventsRequest.Response>()
           .ForwardsTo<SecurityModule>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Expects<SensorEvent<TemperatureReading>>()
           .HandlesWith<TemperatureSensorEventHandler>();

        app.Expects<SensorEvent<AirQualityReading>>()
           .ForwardsTo<VentilationModule>();

        app.Expects<SensorEvent<MotionDetected>>()
           .ForwardsTo<SecurityModule>();

        app.Expects<SensorEvent<DoorOpened>>()
           .ForwardsTo<SecurityModule>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<DownstairsModule>()
           .Uses<UpstairsModule>()
           .Uses<VentilationModule>()
           .Uses<SecurityModule>();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddTychoPersistence<HomeDashboardDbContext>();
    }

    protected override async Task Startup(IServiceProvider app, CancellationToken cancellationToken)
    {
        HomeDashboardDbContext context = app.GetRequiredService<HomeDashboardDbContext>();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);

        // Initialize the ClimateDbContext here once because Climate module has two instances
        await using var climateDb = new ClimateDbContext();
        await climateDb.Database.EnsureDeletedAsync(TestContext.Current.CancellationToken);
        await climateDb.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
    }
}
