using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;

public record CreateProductRequest(string Name, decimal Price) : IRequest<CreateProductRequest.Response>
{
    public record Response(int CreatedProductId);
}