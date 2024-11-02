﻿using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.UseCaseTests.OnlineStore.SUT.Handlers;
using Tycho.UseCaseTests.OnlineStore.SUT.Mappers;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Events;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT;

public class OnlineStoreApp : TychoApp
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

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<CatalogModule>(contract => 
        {
            contract.ForwardAs<
                AddProductToBasketRequest, 
                AddBasketItemRequest, 
                BasketModule>(RequestMapper.Map);
        });

        app.Uses<InventoryModule>()
           .Uses<BasketModule>()
           .Uses<OrderingModule>();
    }

    protected override void MapEvents(IAppEvents app)
    {
        app.Routes<ItemAvailabilityChangedEvent>()
           .ForwardsAs<ProductAvailabilityChangedEvent, CatalogModule>(EventMapper.Map);

        app.Routes<BasketCheckedOutEvent>()
           .ForwardsAs<OrderPlacedEvent, OrderingModule>(EventMapper.Map);

        app.Handles<BasketItemAddedEvent, BasketItemAddedEventHandler>();
    }

    protected override void RegisterServices(IServiceCollection app) { }
}