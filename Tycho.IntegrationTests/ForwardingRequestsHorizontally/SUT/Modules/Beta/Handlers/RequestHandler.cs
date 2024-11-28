using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.Handlers;

internal class RequestHandler(IParent parent)
    : IRequestHandler<Request>
    , IRequestHandler<RequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(Request requestData, CancellationToken cancellationToken)
    {
        return _parent.Execute(requestData, cancellationToken);
    }

    public Task<string> Handle(RequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.Execute<RequestWithResponse, string>(requestData, cancellationToken);
    }
}