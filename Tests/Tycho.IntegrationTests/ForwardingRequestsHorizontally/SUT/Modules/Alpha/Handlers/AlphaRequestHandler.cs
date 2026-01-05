using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaRequestHandler(IParent parent)
    : IRequestHandler<AlphaRequest>
    , IRequestHandler<AlphaRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(AlphaRequest requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(AlphaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync<AlphaRequestWithResponse, string>(requestData, cancellationToken);
    }
}