using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory;

[TychoDefinition]
public partial class InventoryModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<StockItemRequest>()
              .HandlesWith<StockItemRequestHandler>();

        module.Expects<ReserveItemRequest, ReserveItemRequest.Response>()
              .HandlesWith<ReserveItemRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<ItemAvailabilityChangedEvent>()
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<InventoryDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using InventoryDbContext context = app.GetRequiredService<InventoryDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
