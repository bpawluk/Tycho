using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Outgoing;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Mappers;

internal static class RequestMapper
{
    public static AddBasketItemRequest Map(AddProductToBasketRequest requestData)
    {
        return new(
            requestData.CustomerId,
            requestData.ProductId,
            requestData.Quantity,
            requestData.Price);
    }
}