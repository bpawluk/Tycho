using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class ProductAvailabilityChangedEventHandler(CatalogDbContext dbContext) : ITransactionalEventHandler<ProductAvailabilityChangedEvent>
{
    public async Task HandleAsync(EventContext<ProductAvailabilityChangedEvent> context, CancellationToken cancellationToken)
    {
        Product? product = await dbContext.Products.FindAsync([context.Payload.Product], cancellationToken);
        if (product != null)
        {
            var newAvailability = new ProductAvailability(context.Payload.NewQuantity, context.Payload.Version);
            product.UpdateAvailability(newAvailability);
        }
    }
}
