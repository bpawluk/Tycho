using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract.GetOrdersRequest;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Handlers;

internal class GetOrdersRequestHandler : IRequestHandler<GetOrdersRequest, Response>
{
    public Task<Response> Handle(GetOrdersRequest requestData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}