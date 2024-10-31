using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Events;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class AddBasketItemRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddBasketItemRequest>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(AddBasketItemRequest requestData, CancellationToken cancellationToken)
    {
        var basketProvider = new BasketProvider(_unitOfWork);
        var customerBasket = await basketProvider.GetBasket(requestData.CustomerId, cancellationToken);

        var newBasketItem = new BasketItem(requestData.ProductId, requestData.Quantity, requestData.Price);
        customerBasket.Add(newBasketItem);

        var itemAdded = new BasketItemAddedEvent(requestData.CustomerId, requestData.ProductId, requestData.Quantity);
        await _unitOfWork.Publish(itemAdded, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);
    }
}