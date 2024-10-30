using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class CheckoutRequestHandler : IRequestHandler<CheckoutRequest>
{
    public Task Handle(CheckoutRequest requestData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}