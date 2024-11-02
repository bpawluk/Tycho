using Basket = Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;
using Catalog = Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Mappers;

internal static class RequestMapper
{
    public static Basket.AddBasketItemRequest Map(Catalog.AddProductToBasketRequest requestData)
    {
        return new(
            requestData.CustomerId,
            requestData.ProductId,
            requestData.Quantity,
            requestData.Price);
    }
}