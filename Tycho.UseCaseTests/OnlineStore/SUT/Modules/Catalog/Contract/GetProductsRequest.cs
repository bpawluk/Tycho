using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;

public record GetProductsRequest() : IRequest<GetProductsRequest.Response>
{
    public record Response(IReadOnlyList<Product> Products);

    public record Product(
            int Id,
            string Name,
            decimal Price,
            uint Availability);
}