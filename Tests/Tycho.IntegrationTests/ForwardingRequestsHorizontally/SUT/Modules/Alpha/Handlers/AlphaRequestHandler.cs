using Tycho.Requests;
using Tycho.Structure.External;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaRequestHandler(IParentReference parent)
    : IRequestHandler<AlphaRequest>
    , IRequestHandler<AlphaRequestWithResponse, string>
{
    private readonly IParentReference _parent = parent;

    public Task HandleAsync(AlphaRequest requestData, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
        //return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(AlphaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult("Error");
        //return _parent.ExecuteAsync<AlphaRequestWithResponse, string>(requestData, cancellationToken);
    }
}