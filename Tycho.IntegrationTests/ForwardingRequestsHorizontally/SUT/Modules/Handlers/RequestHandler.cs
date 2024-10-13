using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Handlers;

internal class RequestHandler(IParent parent)
    : IRequestHandler<Request>
    , IRequestHandler<RequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(Request requestData, CancellationToken cancellationToken) =>
        _parent.Execute(requestData, cancellationToken);

    public Task<string> Handle(RequestWithResponse requestData, CancellationToken cancellationToken) =>
        _parent.Execute<RequestWithResponse, string>(requestData, cancellationToken);
}
