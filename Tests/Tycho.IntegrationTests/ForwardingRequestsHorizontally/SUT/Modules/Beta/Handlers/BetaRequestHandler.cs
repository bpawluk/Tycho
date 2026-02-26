using Tycho.Requests;
using Tycho.Structure.External;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaRequestHandler(IParent parent)
    : IRequestHandler<BetaRequest>
    , IRequestHandler<BetaRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(BetaRequest requestData, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
        //return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(BetaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult("Error");
        //return _parent.ExecuteAsync<BetaRequestWithResponse, string>(requestData, cancellationToken);
    }
}