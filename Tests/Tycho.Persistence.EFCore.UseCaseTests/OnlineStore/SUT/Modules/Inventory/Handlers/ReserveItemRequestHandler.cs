using Tycho.Transactions;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Outgoing;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Persistence;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming.ReserveItemRequest;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.InventoryModule;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Handlers;

internal class ReserveItemRequestHandler(InventoryDbContext dbContext, IPublisher publisher) : ITransactionalRequestHandler<ReserveItemRequest, Response>
{
    public async Task<Response> HandleAsync(ReserveItemRequest requestData, CancellationToken cancellationToken)
    {
        var item = await dbContext.Items.FindAsync([requestData.ItemId], cancellationToken);
        if (item != null)
        {
            var reserved = item.Reserve(requestData.ReservationCode, requestData.Quantity);
            if (reserved)
            {
                var itemAvailabilityChanged = new ItemAvailabilityChangedEvent(item.Id, item.Availability.Quantity, item.Availability.Version);
                await publisher.PublishAsync(itemAvailabilityChanged, cancellationToken);
            }
            return new Response(reserved);
        }
        return new Response(false);
    }
}