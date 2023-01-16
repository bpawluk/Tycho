using Microsoft.Extensions.DependencyInjection;
using SampleApp.Pricing.Business;
using SampleApp.Pricing.Data;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace SampleApp.Pricing;

// Incoming
public record GetPriceQuery(string ProductId) : IQuery<decimal>;
public record StockLevelChangedEvent(string ProductId, int OldLevel, int NewLevel) : IEvent;

// Outgoing
public record PriceChangedEvent(string ProductId, decimal OldPrice, decimal NewPrice) : IEvent;

public class PricingModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.SubscribesTo<StockLevelChangedEvent>(eventData =>
        {
            services.GetService<IPricingStrategy>()!
                    .AdjustForAvailability(eventData.ProductId, eventData.OldLevel, eventData.NewLevel);
        });

        module.RespondsTo<GetPriceQuery, decimal>(queryData =>
        {
            return services.GetService<IPricesRepository>()!
                           .GetPriceByProductId(queryData.ProductId);
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<PriceChangedEvent>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition submodules, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IPricingStrategy, PricingStrategy>();
        services.AddSingleton<IPricesRepository, PricesRepository>();
    }
}
