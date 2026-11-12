using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.Handlers;

internal class RequestHandler(IBetaModuleParent parent)
    : IRequestHandler<Request>
    , IRequestHandler<RequestWithResponse, string>
{
    private readonly IBetaModuleParent _parent = parent;

    public Task HandleAsync(Request requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(RequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }
}
