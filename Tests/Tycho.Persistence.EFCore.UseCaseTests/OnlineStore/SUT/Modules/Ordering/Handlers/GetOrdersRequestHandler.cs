using Microsoft.EntityFrameworkCore;
using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Persistence;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract.GetOrdersRequest;

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