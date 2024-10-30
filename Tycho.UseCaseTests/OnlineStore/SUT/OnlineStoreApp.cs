using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.UseCaseTests.OnlineStore.SUT.Messaging;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering;

namespace Tycho.UseCaseTests.OnlineStore.SUT;

public class OnlineStoreApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.ForwardsCatalogRequests()
           .ForwardsInventoryRequests()
           .ForwardsBasketRequests()
           .ForwardsOrderingRequests();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<CatalogModule>()
           .Uses<InventoryModule>()
           .Uses<BasketModule>()
           .Uses<OrderingModule>();
    }

    protected override void MapEvents(IAppEvents app)
    {
        app.RoutesInventoryEvents()
           .RoutesBasketEvents();
    }

    protected override void RegisterServices(IServiceCollection app) { }
}