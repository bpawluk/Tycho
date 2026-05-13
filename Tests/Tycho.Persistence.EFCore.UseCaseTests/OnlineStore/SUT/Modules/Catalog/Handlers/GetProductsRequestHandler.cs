using Microsoft.EntityFrameworkCore;
using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming.GetProductsRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class GetProductsRequestHandler(CatalogDbContext dbContext) : IRequestHandler<GetProductsRequest, Response>
{
    public async Task<Response> HandleAsync(GetProductsRequest requestData, CancellationToken cancellationToken)
    {
        var responseProducts = await dbContext.Products
            .Select(p => new Product(p.Id, p.Name, p.Price, p.Availability.Quantity))
            .ToArrayAsync(cancellationToken);
        return new Response(responseProducts);
    }
}