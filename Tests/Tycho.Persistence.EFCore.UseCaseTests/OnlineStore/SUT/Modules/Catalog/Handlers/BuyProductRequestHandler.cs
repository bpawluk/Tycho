using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.CatalogModule;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class BuyProductRequestHandler(CatalogDbContext dbContext, IParent parent) : IRequestHandler<BuyProductRequest>
{
    public async Task HandleAsync(BuyProductRequest requestData, CancellationToken cancellationToken)
    {
        Product? productToBuy = await dbContext.Products.FindAsync([requestData.ProductId], cancellationToken);
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
