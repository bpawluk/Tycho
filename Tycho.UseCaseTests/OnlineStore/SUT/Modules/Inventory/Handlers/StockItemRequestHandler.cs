using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Outgoing;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Handlers;

internal class StockItemRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<StockItemRequest>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(StockItemRequest requestData, CancellationToken cancellationToken)
    {
        var items = _unitOfWork.Set<Item>();

        var item = await items.FindAsync([requestData.ItemId], cancellationToken);
        if (item is null)
        {
            item = new Item(requestData.ItemId, requestData.Quantity);
            items.Add(item);
        }
        else
        {
            item.Stock(requestData.Quantity);
        }

        var itemAvailabilityChanged = new ItemAvailabilityChangedEvent(item.Id, item.Availability.Quantity, item.Availability.Version);
        await _unitOfWork.Publish(itemAvailabilityChanged, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);
    }
}