using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.CreateProductRequest;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class CreateProductRequestHandler : IRequestHandler<CreateProductRequest, Response>
{
    public Task<Response> Handle(CreateProductRequest requestData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}