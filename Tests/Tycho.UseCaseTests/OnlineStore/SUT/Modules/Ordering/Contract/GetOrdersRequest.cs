using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

public record GetOrdersRequest() : IRequest<GetOrdersRequest.Response>
{
    public record Response(IReadOnlyList<Order> Orders);

    public record Order(int Id, int CustomerId, decimal Total);
}