using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Gamma.Handlers;

internal class GammaRequestHandler(IParent parent)
    : IRequestHandler<GammaRequest>
    , IRequestHandler<GammaRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(GammaRequest requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(GammaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync<GammaRequestWithResponse, string>(requestData, cancellationToken);
    }
}