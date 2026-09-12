using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;

public record ReserveItemRequest(string ReservationCode, int ItemId, uint Quantity) : IRequest<ReserveItemRequest.Response>
{
    public record Response(bool ReservationCreated);
}
