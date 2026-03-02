using Tycho.Requests;
using static Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.BetaModule;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaRequestHandler(IParent parent)
    : IRequestHandler<BetaRequest>
    , IRequestHandler<BetaRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(BetaRequest requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(BetaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }
}