using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Handlers;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory;

internal class InventoryModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<StockItemRequest, StockItemRequestHandler>()
              .Handles<ReserveItemRequest, ReserveItemRequest.Response, ReserveItemRequestHandler>()
              .Handles<CompleteReservationRequest, CompleteReservationRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Routes<StockLevelChangedEvent>()
              .Exposes();
    }

    protected override void RegisterServices(IServiceCollection module)
    {

    }
}