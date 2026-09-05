using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class AddBasketItemRequestHandler(BasketDbContext dbContext, IBasketModulePublisher publisher) : ITransactionalRequestHandler<AddBasketItemRequest>
{
    public async Task HandleAsync(AddBasketItemRequest requestData, CancellationToken cancellationToken)
    {
        var basketProvider = new BasketProvider(dbContext);
        Domain.Basket customerBasket = await basketProvider.GetBasket(requestData.CustomerId, cancellationToken);

        var newBasketItem = new BasketItem(requestData.ProductId, requestData.Quantity, requestData.Price);
        customerBasket.Add(newBasketItem);

        var itemAdded = new BasketItemAddedEvent(requestData.CustomerId, requestData.ProductId, requestData.Quantity);
        await publisher.PublishAsync(itemAdded, cancellationToken);
    }
}
