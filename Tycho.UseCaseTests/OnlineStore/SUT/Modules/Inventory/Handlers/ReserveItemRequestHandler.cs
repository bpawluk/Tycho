using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.ReserveItemRequest;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Handlers;

internal class ReserveItemRequestHandler : IRequestHandler<ReserveItemRequest, Response>
{
    public Task<Response> Handle(ReserveItemRequest requestData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}