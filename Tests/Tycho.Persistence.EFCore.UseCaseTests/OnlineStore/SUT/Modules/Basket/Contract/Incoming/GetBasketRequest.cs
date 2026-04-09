using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;

public record GetBasketRequest(int CustomerId) : IRequest<GetBasketRequest.Response>
{
    public record Response(IReadOnlyList<BasketItem> BasketItems);

    public record BasketItem(int ProductId, uint Quantity, decimal Price, string Status);
}