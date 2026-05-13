using Tycho.Events;
using Tycho.Transactions;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Persistence;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Handlers;

internal class OrderPlacedEventHandler(OrderingDbContext dbContext) : ITransactionalEventHandler<OrderPlacedEvent>
{
    public async Task HandleAsync(EventContext<OrderPlacedEvent> context, CancellationToken cancellationToken)
    {
        var newOrder = new Order(context.Payload.CustomerId, context.Payload.Total);
        dbContext.Orders.Add(newOrder);
    }
}