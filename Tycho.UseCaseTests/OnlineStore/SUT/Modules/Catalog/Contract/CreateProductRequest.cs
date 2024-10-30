using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;

public record CreateProductRequest() : IRequest<CreateProductRequest.Response>
{
    public record Response();
}