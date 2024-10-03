using TychoV2.Requests;
using TychoV2.Structure;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules.Handlers;

internal class RequestHandler(IParent parent)
    : IHandle<Request>
    , IHandle<RequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(Request requestData, CancellationToken cancellationToken) => 
        _parent.Execute(requestData, cancellationToken);

    public Task<string> Handle(RequestWithResponse requestData, CancellationToken cancellationToken) =>
        _parent.Execute<RequestWithResponse, string>(requestData, cancellationToken);
}
