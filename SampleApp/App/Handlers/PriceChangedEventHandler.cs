using SampleApp.Catalog;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace SampleApp.App.Handlers;

internal class PriceChangedEventHandler : IEventHandler<Pricing.PriceChangedEvent>
{
    private readonly ISubmodule<CatalogModule> _catalogModule;

    public PriceChangedEventHandler(ISubmodule<CatalogModule> catalogModule)
    {
        _catalogModule = catalogModule;
    }

    public Task Handle(Pricing.PriceChangedEvent eventData, CancellationToken cancellationToken)
    {
        _catalogModule.PublishEvent(
            new Catalog.PriceChangedEvent(
                eventData.ProductId,
                eventData.OldPrice,
                eventData.NewPrice),
            cancellationToken);

        return Task.CompletedTask;
    }
}
