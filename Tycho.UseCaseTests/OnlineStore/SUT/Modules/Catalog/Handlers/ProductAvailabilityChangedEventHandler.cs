using Tycho.Events;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class ProductAvailabilityChangedEventHandler(IUnitOfWork unitOfWork) : IEventHandler<ProductAvailabilityChangedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(ProductAvailabilityChangedEvent eventData, CancellationToken cancellationToken)
    {
        var products = _unitOfWork.Set<Product>();
        var product = await products.FindAsync([eventData.Product], cancellationToken);
        if (product != null) 
        {
            var newAvailability = new ProductAvailability(eventData.NewQuantity, eventData.Version);
            product.UpdateAvailability(newAvailability);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}