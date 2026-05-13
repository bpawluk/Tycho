using Tycho.Transactions;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Persistence;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.BasketModule;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class AddBasketItemRequestHandler(BasketDbContext dbContext, IPublisher publisher) : ITransactionalRequestHandler<AddBasketItemRequest>
{
    public async Task HandleAsync(AddBasketItemRequest requestData, CancellationToken cancellationToken)
    {
        var basketProvider = new BasketProvider(dbContext);
        var customerBasket = await basketProvider.GetBasket(requestData.CustomerId, cancellationToken);

        var newBasketItem = new BasketItem(requestData.ProductId, requestData.Quantity, requestData.Price);
        customerBasket.Add(newBasketItem);

        var itemAdded = new BasketItemAddedEvent(requestData.CustomerId, requestData.ProductId, requestData.Quantity);
        await publisher.PublishAsync(itemAdded, cancellationToken);
    }
}