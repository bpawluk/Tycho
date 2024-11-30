﻿using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Outgoing;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog;

internal class CatalogModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<CreateProductRequest, CreateProductRequest.Response, CreateProductRequestHandler>()
              .Handles<GetProductsRequest, GetProductsRequest.Response, GetProductsRequestHandler>()
              .Handles<BuyProductRequest, BuyProductRequestHandler>();

        module.Requires<AddProductToBasketRequest>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Handles<ProductAvailabilityChangedEvent, ProductAvailabilityChangedEventHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<CatalogDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<CatalogDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}