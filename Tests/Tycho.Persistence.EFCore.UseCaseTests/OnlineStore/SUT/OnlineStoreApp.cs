using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Mappers;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT;

[TychoDefinition]
public partial class OnlineStoreApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Forwards<CreateProductRequest, CreateProductRequest.Response, CatalogModule>()
           .Forwards<GetProductsRequest, GetProductsRequest.Response, CatalogModule>()
           .Forwards<BuyProductRequest, CatalogModule>();

        app.Forwards<StockItemRequest, InventoryModule>();

        app.Forwards<GetBasketRequest, GetBasketRequest.Response, BasketModule>()
           .Forwards<CheckoutRequest, BasketModule>();

        app.Forwards<GetOrdersRequest, GetOrdersRequest.Response, OrderingModule>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Routes<ItemAvailabilityChangedEvent>()
           .ForwardsAs<ProductAvailabilityChangedEvent, CatalogModule>(EventMapper.Map);

        app.Routes<BasketCheckedOutEvent>()
           .ForwardsAs<OrderPlacedEvent, OrderingModule>(EventMapper.Map);

        app.Handles<BasketItemAddedEvent, BasketItemAddedEventHandler>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<CatalogModule>(outgoingRequests =>
        {
            outgoingRequests.ForwardAs<
                AddProductToBasketRequest,
                AddBasketItemRequest,
                BasketModule>(RequestMapper.Map);
        });

        app.Uses<InventoryModule>()
           .Uses<BasketModule>()
           .Uses<OrderingModule>();
    }

    protected override void RegisterServices(IServiceCollection app) { }
}
