using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Outgoing;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.CatalogModule;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class BuyProductRequestHandler(IParent parent, IUnitOfWork unitOfWork) : IRequestHandler<BuyProductRequest>
{
    private readonly IParent _parent = parent;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task HandleAsync(BuyProductRequest requestData, CancellationToken cancellationToken)
    {
        var products = _unitOfWork.Set<Product>();
        var productToBuy = await products.FindAsync([requestData.ProductId], cancellationToken);
        if (productToBuy != null && productToBuy.IsEnoughAvailable(requestData.Quantity))
        {
            var addToBasketRequest = new AddProductToBasketRequest(
                requestData.CustomerId,
                requestData.ProductId,
                requestData.Quantity,
                productToBuy.Price);
            await _parent.ExecuteAsync(addToBasketRequest, cancellationToken);
        }
    }
}