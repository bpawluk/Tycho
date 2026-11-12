using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaRequestHandler(IAlphaModuleParent parent)
    : IRequestHandler<AlphaRequest>
    , IRequestHandler<AlphaRequestWithResponse, string>
{
    private readonly IAlphaModuleParent _parent = parent;

    public Task HandleAsync(AlphaRequest requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(AlphaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }
}
