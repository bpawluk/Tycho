using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaRequestHandler(IBetaModuleParent parent)
    : IRequestHandler<BetaRequest>
    , IRequestHandler<BetaRequestWithResponse, string>
{
    private readonly IBetaModuleParent _parent = parent;

    public Task HandleAsync(BetaRequest requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(BetaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }
}
