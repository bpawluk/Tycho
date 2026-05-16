using Tycho.Requests;
using static Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules.GammaModule;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules.Handlers;

internal class GammaRequestHandler(IParent parent)
    : IRequestHandler<GammaRequest>
    , IRequestHandler<GammaRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(GammaRequest requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }

    public Task<string> HandleAsync(GammaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.ExecuteAsync(requestData, cancellationToken);
    }
}
