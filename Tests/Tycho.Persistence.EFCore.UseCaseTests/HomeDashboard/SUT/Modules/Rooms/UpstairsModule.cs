using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Handlers;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms;

[TychoDefinition]
public partial class UpstairsModule : TychoModule
{
    private readonly RoomSettings _settings = new() { Room = "upstairs" };

    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<RegisterTemperatureSensorEventRequest>()
              .HandlesWith<UpstairsTemperatureSensorEventRequestHandler>();

        module.Expects<GetTemperatureReadingsRequest, GetTemperatureReadingsRequest.Response>()
              .ForwardsTo<ClimateModule>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<SensorEvent<TemperatureReading>>()
              .ForwardsTo<ClimateModule>();
    }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<ClimateModule>(_settings);
    }

    protected override void RegisterServices(IServiceCollection module) { }
}
