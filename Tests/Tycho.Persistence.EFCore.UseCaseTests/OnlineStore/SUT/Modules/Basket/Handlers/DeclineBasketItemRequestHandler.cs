using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class DeclineBasketItemRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeclineBasketItemRequest>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task HandleAsync(DeclineBasketItemRequest requestData, CancellationToken cancellationToken)
    {
        var basketProvider = new BasketProvider(_unitOfWork);
        var customerBasket = await basketProvider.GetBasket(requestData.CustomerId, cancellationToken);
        customerBasket.DeclineItem(requestData.ProductId);
        await _unitOfWork.SaveChanges(cancellationToken);
    }
}