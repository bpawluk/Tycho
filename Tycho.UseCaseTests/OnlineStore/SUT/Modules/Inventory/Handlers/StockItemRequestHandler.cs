using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Handlers;

internal class StockItemRequestHandler : IRequestHandler<StockItemRequest>
{
    public Task Handle(StockItemRequest requestData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}