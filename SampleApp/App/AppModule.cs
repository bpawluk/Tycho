using Microsoft.Extensions.DependencyInjection;
using SampleApp.App.Handlers;
using SampleApp.Catalog;
using SampleApp.Catalog.Model;
using SampleApp.Inventory;
using SampleApp.Pricing;
using System;
using System.Collections.Generic;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace SampleApp.App;

// Incoming
public record BuyProductCommand(string ProductId, int Amount) : ICommand;

// Outgoing
// - no outgoing messages specific to this module

public sealed class AppModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.RespondsTo<GetProductsQuery, IEnumerable<Product>, GetProductsQueryHandler>()
              .RespondsTo<FindProductQuery, Product, FindProductQueryHandler>()
              .Executes<BuyProductCommand, BuyProductCommandHandler>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<CatalogModule>();

        module.AddSubmodule<InventoryModule>((IOutboxConsumer thisConsumer) =>
        {
            thisConsumer.HandleEvent<Inventory.StockLevelChangedEvent, StockLevelChangedEventHandler>();
        });

        module.AddSubmodule<PricingModule>((IOutboxConsumer thisConsumer) =>
        {
            thisConsumer.HandleEvent<Pricing.PriceChangedEvent, PriceChangedEventHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection services) { }
}
