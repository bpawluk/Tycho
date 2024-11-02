using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.GetProductsRequest;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class GetProductsRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetProductsRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(GetProductsRequest requestData, CancellationToken cancellationToken)
    {
        var products = _unitOfWork.Set<Domain.Product>();
        var responseProducts = await products
            .Select(p => new Product(p.Id, p.Name, p.Price, p.Availability.Quantity))
            .ToListAsync(cancellationToken);
        return new Response(responseProducts);
    }
}