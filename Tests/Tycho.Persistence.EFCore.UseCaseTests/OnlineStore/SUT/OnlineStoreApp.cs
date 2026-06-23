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
        app.Expects<CreateProductRequest, CreateProductRequest.Response>()
           .ForwardsTo<CatalogModule>();

        app.Expects<GetProductsRequest, GetProductsRequest.Response>()
           .ForwardsTo<CatalogModule>();

        app.Expects<BuyProductRequest>()
           .ForwardsTo<CatalogModule>();

        app.Expects<StockItemRequest>()
           .ForwardsTo<InventoryModule>();

        app.Expects<GetBasketRequest, GetBasketRequest.Response>()
           .ForwardsTo<BasketModule>();

        app.Expects<CheckoutRequest>()
           .ForwardsTo<BasketModule>();

        app.Expects<GetOrdersRequest, GetOrdersRequest.Response>()
           .ForwardsTo<OrderingModule>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Expects<ItemAvailabilityChangedEvent>()
           .MapsTo<ProductAvailabilityChangedEvent>(EventMapper.Map)
           .ForwardsTo<CatalogModule>();

        app.Expects<BasketCheckedOutEvent>()
           .MapsTo<OrderPlacedEvent>(EventMapper.Map)
           .ForwardsTo<OrderingModule>();

        app.Expects<BasketItemAddedEvent>()
           .HandlesWith<BasketItemAddedEventHandler>();
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
