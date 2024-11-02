using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract.GetOrdersRequest;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Handlers;

internal class GetOrdersRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrdersRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(GetOrdersRequest requestData, CancellationToken cancellationToken)
    {
        var orders = _unitOfWork.Set<Domain.Order>();
        var result = await orders
            .Select(order => new Order(order.Id, order.CustomerId, order.Total))
            .ToListAsync(cancellationToken);
        return new Response(result);
    }
}