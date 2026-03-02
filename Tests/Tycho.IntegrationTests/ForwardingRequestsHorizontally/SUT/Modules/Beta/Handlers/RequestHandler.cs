using Tycho.Requests;
using static Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.BetaModule;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.Handlers;

internal class RequestHandler(IParent parent)
    : IRequestHandler<Request>
    , IRequestHandler<RequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(Request requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(RequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }
}