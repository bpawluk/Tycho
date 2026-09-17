using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming.CreateProductRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class CreateProductRequestHandler(CatalogDbContext dbContext) : IRequestHandler<CreateProductRequest, Response>
{
    public async Task<Response> HandleAsync(CreateProductRequest requestData, CancellationToken cancellationToken)
    {
        var newProduct = new Product(requestData.Name, requestData.Price);
        dbContext.Products.Add(newProduct);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new Response(newProduct.Id);
    }
}
