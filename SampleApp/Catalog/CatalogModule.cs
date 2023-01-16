using Microsoft.Extensions.DependencyInjection;
using SampleApp.Catalog.Model;
using SampleApp.Catalog.Persistence;
using System;
using System.Collections.Generic;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace SampleApp.Catalog;

// Incoming
public record GetProductsQuery() : IQuery<IEnumerable<Product>>;
public record FindProductQuery(string ProductName) : IQuery<Product>;
public record PriceChangedEvent(string ProductId, decimal OldPrice, decimal NewPrice) : IEvent;
public record StockLevelChangedEvent(string ProductId, int OldLevel, int NewLevel) : IEvent;

// Outgoing
// - no outgoing messages

public class CatalogModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var repository = services.GetService<IProductsRepository>()!;

        module.RespondsTo<GetProductsQuery, IEnumerable<Product>>(q => repository.GetProducts())
              .RespondsTo<FindProductQuery, Product>(q => repository.FindProduct(q.ProductName)!)
              .SubscribesTo<PriceChangedEvent>(e => repository.UpdatePrice(e.ProductId, e.NewPrice))
              .SubscribesTo<StockLevelChangedEvent>(e => repository.UpdateStockLevel(e.ProductId, e.NewLevel));
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }

    protected override void IncludeSubmodules(ISubstructureDefinition submodules, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IProductsRepository, ProductsRepository>();
    }
}
