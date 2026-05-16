using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Persistence;
using Tycho.Transactions;
using static Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.BasketModule;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

internal class CheckoutRequestHandler(BasketDbContext dbContext, IPublisher publisher) : ITransactionalRequestHandler<CheckoutRequest>
{
    public async Task HandleAsync(CheckoutRequest requestData, CancellationToken cancellationToken)
    {
        var basketProvider = new BasketProvider(dbContext);
        Domain.Basket customerBasket = await basketProvider.GetBasket(requestData.CustomerId, cancellationToken);
        customerBasket.Checkout();

        var basketCheckedOutEvent = new BasketCheckedOutEvent(customerBasket.CustomerId, customerBasket.TotalAmount);
        await publisher.PublishAsync(basketCheckedOutEvent, cancellationToken);
    }
}
