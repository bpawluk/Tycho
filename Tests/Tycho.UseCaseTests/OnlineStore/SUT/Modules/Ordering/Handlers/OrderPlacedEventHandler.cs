using Tycho.Events;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Handlers;

internal class OrderPlacedEventHandler(IUnitOfWork unitOfWork) : IEventHandler<OrderPlacedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task HandleAsync(EventContext<OrderPlacedEvent> context, CancellationToken cancellationToken)
    {
        var orders = _unitOfWork.Set<Order>();
        var newOrder = new Order(context.Payload.CustomerId, context.Payload.Total);
        orders.Add(newOrder);
        await _unitOfWork.SaveChanges(cancellationToken);
    }
}