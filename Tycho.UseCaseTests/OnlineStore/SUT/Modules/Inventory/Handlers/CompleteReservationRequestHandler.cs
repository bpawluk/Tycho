using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Handlers;

internal class CompleteReservationRequestHandler : IRequestHandler<CompleteReservationRequest>
{
    public Task Handle(CompleteReservationRequest requestData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}