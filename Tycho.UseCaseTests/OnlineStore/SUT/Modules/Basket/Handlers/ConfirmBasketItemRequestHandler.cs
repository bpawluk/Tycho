using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class ConfirmBasketItemRequestHandler : IRequestHandler<ConfirmBasketItemRequest>
{
    public Task Handle(ConfirmBasketItemRequest requestData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}