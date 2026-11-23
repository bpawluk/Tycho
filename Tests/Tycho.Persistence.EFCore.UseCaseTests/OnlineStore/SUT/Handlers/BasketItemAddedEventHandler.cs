using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Handlers;

internal class BasketItemAddedEventHandler(IInventoryModule inventoryModule, IBasketModule basketModule) : IEventHandler<BasketItemAddedEvent>
{
    public async Task HandleAsync(EventContext<BasketItemAddedEvent> context, CancellationToken cancellationToken)
    {
        string reservationCode = $"{context.Payload.CustomerId}-{context.Payload.ProductId}";
        var reserveItemRequest = new ReserveItemRequest(reservationCode, context.Payload.ProductId, context.Payload.Quantity);

        ReserveItemRequest.Response response = await inventoryModule.ExecuteAsync(reserveItemRequest, cancellationToken);

        if (response.ReservationCreated)
        {
            var confirmBasketItemRequest = new ConfirmBasketItemRequest(context.Payload.CustomerId, context.Payload.ProductId);
            await basketModule.ExecuteAsync(confirmBasketItemRequest, cancellationToken);
        }
        else
        {
            var declineBasketItemRequest = new DeclineBasketItemRequest(context.Payload.CustomerId, context.Payload.ProductId);
            await basketModule.ExecuteAsync(declineBasketItemRequest, cancellationToken);
        }
    }
}
