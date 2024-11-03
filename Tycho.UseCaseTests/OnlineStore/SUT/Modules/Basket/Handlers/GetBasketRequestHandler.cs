using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming.GetBasketRequest;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class GetBasketRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetBasketRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(GetBasketRequest requestData, CancellationToken cancellationToken)
    {
        var basketProvider = new BasketProvider(_unitOfWork);
        var customerBasket = await basketProvider.GetBasket(requestData.CustomerId, cancellationToken);
        var basketItems = customerBasket.Items
            .Select(item => new GetBasketRequest.BasketItem(
                item.ProductId,
                item.Quantity,
                item.Price,
                item.Status.ToString()))
            .ToArray();
        return new(basketItems);
    }
}