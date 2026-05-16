using Tycho.Requests;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;
using static Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.CatalogModule;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class BuyProductRequestHandler(CatalogDbContext dbContext, IParent parent) : IRequestHandler<BuyProductRequest>
{
    public async Task HandleAsync(BuyProductRequest requestData, CancellationToken cancellationToken)
    {
        var productToBuy = await dbContext.Products.FindAsync([requestData.ProductId], cancellationToken);
        if (productToBuy != null && productToBuy.IsEnoughAvailable(requestData.Quantity))
        {
            var addToBasketRequest = new AddProductToBasketRequest(
                requestData.CustomerId,
                requestData.ProductId,
                requestData.Quantity,
                productToBuy.Price);
            await parent.ExecuteAsync(addToBasketRequest, cancellationToken);
        }
    }
}
