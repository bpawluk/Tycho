using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class AddBasketItemRequestHandler : IRequestHandler<AddBasketItemRequest>
{
    public Task Handle(AddBasketItemRequest requestData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}