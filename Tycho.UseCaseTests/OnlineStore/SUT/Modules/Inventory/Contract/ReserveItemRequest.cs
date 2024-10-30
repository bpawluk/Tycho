using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;

public record ReserveItemRequest() : IRequest<ReserveItemRequest.Response>
{
    public record Response();
}