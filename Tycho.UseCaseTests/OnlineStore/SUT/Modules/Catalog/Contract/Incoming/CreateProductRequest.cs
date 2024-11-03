using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;

public record CreateProductRequest(string Name, decimal Price) : IRequest<CreateProductRequest.Response>
{
    public record Response(int CreatedProductId);
}