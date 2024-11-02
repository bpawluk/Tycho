using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Events;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class CheckoutRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<CheckoutRequest>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(CheckoutRequest requestData, CancellationToken cancellationToken)
    {
        var basketProvider = new BasketProvider(_unitOfWork);
        var customerBasket = await basketProvider.GetBasket(requestData.CustomerId, cancellationToken);
        customerBasket.Checkout();

        var basketCheckedOutEvent = new BasketCheckedOutEvent(
            requestData.CustomerId, 
            customerBasket.Items
                .Select(item => new BasketCheckedOutEvent.BasketItem(item.ProductId, item.Quantity, item.Price))
                .ToArray());
        await _unitOfWork.Publish(basketCheckedOutEvent, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);
    }
}