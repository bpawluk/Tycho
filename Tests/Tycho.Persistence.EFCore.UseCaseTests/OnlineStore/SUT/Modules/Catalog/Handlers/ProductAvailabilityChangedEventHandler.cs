using Tycho.Events;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class ProductAvailabilityChangedEventHandler(IUnitOfWork unitOfWork) : IEventHandler<ProductAvailabilityChangedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task HandleAsync(EventContext<ProductAvailabilityChangedEvent> context, CancellationToken cancellationToken)
    {
        var products = _unitOfWork.Set<Product>();
        var product = await products.FindAsync([context.Payload.Product], cancellationToken);
        if (product != null) 
        {
            var newAvailability = new ProductAvailability(context.Payload.NewQuantity, context.Payload.Version);
            product.UpdateAvailability(newAvailability);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}