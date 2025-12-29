using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules.Handlers;

internal class GammaRequestHandler(IParent parent)
    : IRequestHandler<GammaRequest>
    , IRequestHandler<GammaRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(GammaRequest requestData, CancellationToken cancellationToken)
    {
        return _parent.Execute(requestData, cancellationToken);
    }

    public Task<string> Handle(GammaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.Execute<GammaRequestWithResponse, string>(requestData, cancellationToken);
    }
}