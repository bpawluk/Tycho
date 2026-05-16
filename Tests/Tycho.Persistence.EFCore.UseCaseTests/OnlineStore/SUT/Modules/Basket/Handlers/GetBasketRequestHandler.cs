using Tycho.Requests;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Persistence;
using static Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming.GetBasketRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class GetBasketRequestHandler(BasketDbContext dbContext) : IRequestHandler<GetBasketRequest, Response>
{
    public async Task<Response> HandleAsync(GetBasketRequest requestData, CancellationToken cancellationToken)
    {
        var basketProvider = new BasketProvider(dbContext);
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
