using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Handlers;

internal class OrderPlacedEventHandler(OrderingDbContext dbContext) : ITransactionalEventHandler<OrderPlacedEvent>
{
    public async Task HandleAsync(EventContext<OrderPlacedEvent> context, CancellationToken cancellationToken)
    {
        var newOrder = new Order(context.Payload.CustomerId, context.Payload.Total);
        dbContext.Orders.Add(newOrder);
    }
}
