using Tycho.Events;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Handlers;

internal class OrderPlacedEventHandler(IUnitOfWork unitOfWork) : IEventHandler<OrderPlacedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(OrderPlacedEvent eventData, CancellationToken cancellationToken)
    {
        var orders = _unitOfWork.Set<Order>();

        var newOrder = new Order(
            eventData.CustomerId, 
            eventData.Items.Sum(item => item.Price * item.Quantity));
        orders.Add(newOrder);

        await _unitOfWork.SaveChanges(cancellationToken);
    }
}