using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Persistence;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class DeclineBasketItemRequestHandler(BasketDbContext dbContext) : IRequestHandler<DeclineBasketItemRequest>
{
    public async Task HandleAsync(DeclineBasketItemRequest requestData, CancellationToken cancellationToken)
    {
        var basketProvider = new BasketProvider(dbContext);
        Domain.Basket customerBasket = await basketProvider.GetBasket(requestData.CustomerId, cancellationToken);
        customerBasket.DeclineItem(requestData.ProductId);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
