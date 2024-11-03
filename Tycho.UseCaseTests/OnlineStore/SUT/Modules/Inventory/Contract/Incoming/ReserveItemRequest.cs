using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;

public record ReserveItemRequest(string ReservationCode, int ItemId, uint Quantity) : IRequest<ReserveItemRequest.Response>
{
    public record Response(bool ReservationCreated);
}