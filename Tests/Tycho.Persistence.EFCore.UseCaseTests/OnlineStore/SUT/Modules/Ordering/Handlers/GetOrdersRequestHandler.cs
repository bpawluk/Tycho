using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract.GetOrdersRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Handlers;

internal class GetOrdersRequestHandler(OrderingDbContext dbContext) : IRequestHandler<GetOrdersRequest, Response>
{
    public async Task<Response> HandleAsync(GetOrdersRequest requestData, CancellationToken cancellationToken)
    {
        var result = await dbContext.Orders
            .Select(order => new Order(order.Id, order.CustomerId, order.Total))
            .ToArrayAsync(cancellationToken);
        return new Response(result);
    }
}
