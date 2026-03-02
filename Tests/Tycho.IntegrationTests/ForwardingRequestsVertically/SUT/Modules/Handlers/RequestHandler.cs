using Tycho.Requests;
using static Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules.GammaModule;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules.Handlers;

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