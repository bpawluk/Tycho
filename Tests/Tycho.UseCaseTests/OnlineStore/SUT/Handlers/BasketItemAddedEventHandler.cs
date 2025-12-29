using Tycho.Events;
using Tycho.Structure;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Handlers;

internal class BasketItemAddedEventHandler(
    IModule<InventoryModule> inventoryModule,
    IModule<BasketModule> basketModule) 
    : IEventHandler<BasketItemAddedEvent>
{
    private readonly IModule<InventoryModule> _inventoryModule = inventoryModule;
    private readonly IModule<BasketModule> _basketModule = basketModule;

    public async Task Handle(BasketItemAddedEvent eventData, CancellationToken cancellationToken)
    {
        var reservationCode = $"{eventData.CustomerId}-{eventData.ProductId}";
        var reserveItemRequest = new ReserveItemRequest(reservationCode, eventData.ProductId, eventData.Quantity);

        var response = await _inventoryModule.Execute<ReserveItemRequest, ReserveItemRequest.Response>(
            reserveItemRequest, cancellationToken);

        if (response.ReservationCreated)
        {
            var confirmBasketItemRequest = new ConfirmBasketItemRequest(eventData.CustomerId, eventData.ProductId);
            await _basketModule.Execute(confirmBasketItemRequest, cancellationToken);
        }
        else
        {
            var declineBasketItemRequest = new DeclineBasketItemRequest(eventData.CustomerId, eventData.ProductId);
            await _basketModule.Execute(declineBasketItemRequest, cancellationToken);
        }
    }
}