using TychoV2.Requests;
using TychoV2.Structure;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaInRequestHandler(IParent parent)
    : IHandle<BetaInRequest>
    , IHandle<BetaInRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(BetaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.Execute(new BetaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(BetaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.Execute<BetaOutRequestWithResponse, string>(
            new BetaOutRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
