using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaRequestHandler(IParent parent)
    : IRequestHandler<AlphaRequest>
    , IRequestHandler<AlphaRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(AlphaRequest requestData, CancellationToken cancellationToken)
    {
        return _parent.Execute(requestData, cancellationToken);
    }

    public Task<string> Handle(AlphaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _parent.Execute<AlphaRequestWithResponse, string>(requestData, cancellationToken);
    }
}