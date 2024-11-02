using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Handlers;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Persistence;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory;

internal class InventoryModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<StockItemRequest, StockItemRequestHandler>()
              .Handles<ReserveItemRequest, ReserveItemRequest.Response, ReserveItemRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Routes<ItemAvailabilityChangedEvent>()
              .Exposes();
    }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<InventoryDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<InventoryDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}