using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

public record GetOrdersRequest() : IRequest<GetOrdersRequest.Response>
{
    public record Response();
}