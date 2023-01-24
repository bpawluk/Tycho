using Microsoft.Extensions.DependencyInjection;
using SampleApp.App.Contract;
using SampleApp.App.Handlers;
using SampleApp.Catalog;
using SampleApp.Catalog.Model;
using SampleApp.Inventory;
using SampleApp.Pricing;
using System;
using System.Collections.Generic;
using Tycho;
using Tycho.Contract;
using Tycho.Structure;

using static SampleApp.App.Contract.Mapper;

namespace SampleApp.App;

public sealed class AppModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.Executes<BuyProductCommand, BuyProductCommandHandler>()
              .Forwards<GetProductsQuery, IEnumerable<Product>, CatalogModule>()
              .Forwards<FindProductQuery, Product, CatalogModule>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<CatalogModule>();

        module.AddSubmodule<InventoryModule>((IOutboxConsumer thisConsumer) =>
        {
            thisConsumer.PassOn<Inventory.StockLevelChangedEvent, Pricing.StockLevelChangedEvent, PricingModule>(
                MapToPricing);

            thisConsumer.PassOn<Inventory.StockLevelChangedEvent, Catalog.StockLevelChangedEvent, CatalogModule>(
                MapToCatalog);
        });

        module.AddSubmodule<PricingModule>((IOutboxConsumer thisConsumer) =>
        {
            thisConsumer.PassOn<Pricing.PriceChangedEvent, Catalog.PriceChangedEvent, CatalogModule>(
                MapToCatalog);
        });
    }

    protected override void RegisterServices(IServiceCollection services) { }
}
