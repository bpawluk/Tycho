using Tycho.Events;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class ProductAvailabilityChangedEventHandler : IEventHandler<ProductAvailabilityChangedEvent>
{
    public Task Handle(ProductAvailabilityChangedEvent eventData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}