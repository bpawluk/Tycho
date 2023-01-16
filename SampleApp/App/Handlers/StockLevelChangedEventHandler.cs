using SampleApp.Catalog;
using SampleApp.Pricing;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace SampleApp.App.Handlers;

internal class StockLevelChangedEventHandler : IEventHandler<Inventory.StockLevelChangedEvent>
{
    private readonly ISubmodule<CatalogModule> _catalogModule;
    private readonly ISubmodule<PricingModule> _pricingModule;

    public StockLevelChangedEventHandler(ISubmodule<CatalogModule> catalogModule, ISubmodule<PricingModule> pricingModule)
    {
        _catalogModule = catalogModule;
        _pricingModule = pricingModule;
    }

    public Task Handle(Inventory.StockLevelChangedEvent eventData, CancellationToken cancellationToken)
    {
        _catalogModule.PublishEvent(
            new Catalog.StockLevelChangedEvent(
                eventData.ProductId,
                eventData.PreviousLevel,
                eventData.CurrentLevel),
            cancellationToken);

        _pricingModule.PublishEvent(
            new Pricing.StockLevelChangedEvent(
                eventData.ProductId,
                eventData.PreviousLevel,
                eventData.CurrentLevel),
            cancellationToken);

        return Task.CompletedTask;
    }
}
