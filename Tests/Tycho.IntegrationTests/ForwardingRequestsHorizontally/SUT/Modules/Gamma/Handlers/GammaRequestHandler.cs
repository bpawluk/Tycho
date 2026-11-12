using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Gamma.Handlers;

internal class GammaRequestHandler(IGammaModuleParent parent)
    : IRequestHandler<GammaRequest>
    , IRequestHandler<GammaRequestWithResponse, string>
{
    private readonly IGammaModuleParent _parent = parent;

    public Task HandleAsync(GammaRequest requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(GammaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }
}
