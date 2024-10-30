using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class DeclineBasketItemRequestHandler : IRequestHandler<DeclineBasketItemRequest>
{
    public Task Handle(DeclineBasketItemRequest requestData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}