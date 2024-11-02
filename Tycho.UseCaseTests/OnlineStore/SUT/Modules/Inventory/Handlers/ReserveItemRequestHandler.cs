using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.ReserveItemRequest;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Handlers;

internal class ReserveItemRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<ReserveItemRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(ReserveItemRequest requestData, CancellationToken cancellationToken)
    {
        var items = _unitOfWork.Set<Item>();

        var item = await items.FindAsync([requestData.ItemId], cancellationToken);
        if (item != null)
        {
            var reserved = item.Reserve(requestData.ReservationCode, requestData.Quantity);
            if (reserved)
            {
                var itemAvailabilityChanged = new ItemAvailabilityChangedEvent(item.Id, item.Availability.Quantity, item.Availability.Version);
                await _unitOfWork.Publish(itemAvailabilityChanged, cancellationToken);

                await _unitOfWork.SaveChanges(cancellationToken);
            }
            return new Response(reserved);
        }

        return new Response(false);
    }
}