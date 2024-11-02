using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.Structure;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class BuyProductRequestHandler(IParent parent, IUnitOfWork unitOfWork) : IRequestHandler<BuyProductRequest>
{
    private readonly IParent _parent = parent;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(BuyProductRequest requestData, CancellationToken cancellationToken)
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
            await _parent.Execute(addToBasketRequest, cancellationToken);
        }
    }
}