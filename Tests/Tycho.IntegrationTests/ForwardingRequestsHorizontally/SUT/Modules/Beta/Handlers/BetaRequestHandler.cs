using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaRequestHandler(IParent parent)
    : IRequestHandler<BetaRequest>
    , IRequestHandler<BetaRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(BetaRequest requestData, CancellationToken cancellationToken)
    {
        return _parent.Execute(requestData, cancellationToken);
    }

    public Task<string> Handle(BetaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.Execute<BetaRequestWithResponse, string>(requestData, cancellationToken);
    }
}