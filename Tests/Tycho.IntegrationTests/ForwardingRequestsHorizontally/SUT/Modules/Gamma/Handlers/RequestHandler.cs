using Tycho.Requests;
using Tycho.Structure.External;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Gamma.Handlers;

internal class RequestHandler(IParentReference parent)
    : IRequestHandler<Request>
    , IRequestHandler<RequestWithResponse, string>
{
    private readonly IParentReference _parent = parent;

    public Task HandleAsync(Request requestData, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
        //return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(RequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult("Error");
        //return _parent.ExecuteAsync<RequestWithResponse, string>(requestData, cancellationToken);
    }
}