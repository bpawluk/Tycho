using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.GetProductsRequest;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class GetProductsRequestHandler : IRequestHandler<GetProductsRequest, Response>
{
    public Task<Response> Handle(GetProductsRequest requestData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}