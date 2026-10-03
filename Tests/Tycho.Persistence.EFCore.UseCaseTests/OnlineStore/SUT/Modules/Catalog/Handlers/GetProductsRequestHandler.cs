using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming.GetProductsRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class GetProductsRequestHandler(CatalogDbContext dbContext) : IRequestHandler<GetProductsRequest, Response>
{
    public async Task<Response> HandleAsync(GetProductsRequest requestData, CancellationToken cancellationToken)
    {
        Product[] responseProducts = await dbContext.Products
            .Select(p => new Product(p.Id, p.Name, p.Price, p.Availability.Quantity))
            .ToArrayAsync(cancellationToken);
        return new Response(responseProducts);
    }
}
