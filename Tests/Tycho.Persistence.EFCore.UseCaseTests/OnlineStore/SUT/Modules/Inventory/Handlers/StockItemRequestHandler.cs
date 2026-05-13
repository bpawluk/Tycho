using Tycho.Transactions;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Outgoing;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Persistence;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.InventoryModule;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Handlers;

internal class StockItemRequestHandler(InventoryDbContext dbContext, IPublisher publisher) : ITransactionalRequestHandler<StockItemRequest>
{
    public async Task HandleAsync(StockItemRequest requestData, CancellationToken cancellationToken)
    {
        var item = await dbContext.Items.FindAsync([requestData.ItemId], cancellationToken);
        if (item is null)
        {
            item = new Item(requestData.ItemId, requestData.Quantity);
            dbContext.Items.Add(item);
        }
        else
        {
            item.Stock(requestData.Quantity);
        }

        var itemAvailabilityChanged = new ItemAvailabilityChangedEvent(item.Id, item.Availability.Quantity, item.Availability.Version);
        await publisher.PublishAsync(itemAvailabilityChanged, cancellationToken);
    }
}