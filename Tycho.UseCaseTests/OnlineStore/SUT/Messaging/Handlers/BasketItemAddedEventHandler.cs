using Tycho.Events;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Events;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging.Handlers;

internal class BasketItemAddedEventHandler : IEventHandler<BasketItemAddedEvent>
{
    public Task Handle(BasketItemAddedEvent eventData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
