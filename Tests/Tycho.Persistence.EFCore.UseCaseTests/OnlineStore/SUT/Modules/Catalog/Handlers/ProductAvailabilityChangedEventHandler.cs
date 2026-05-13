using Tycho.Events;
using Tycho.Transactions;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class ProductAvailabilityChangedEventHandler(CatalogDbContext dbContext) : ITransactionalEventHandler<ProductAvailabilityChangedEvent>
{
    public async Task HandleAsync(EventContext<ProductAvailabilityChangedEvent> context, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync([context.Payload.Product], cancellationToken);
        if (product != null) 
        {
            var newAvailability = new ProductAvailability(context.Payload.NewQuantity, context.Payload.Version);
            product.UpdateAvailability(newAvailability);
        }
    }
}