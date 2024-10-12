using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta;
using TychoV2.Requests;
using TychoV2.Structure;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha.Handlers;

internal class GammaOutRequestHandler(IParent parent)
    : IRequestHandler<BetaOutRequest>
    , IRequestHandler<BetaOutRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(BetaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.Execute(new AlphaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(BetaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.Execute<AlphaOutRequestWithResponse, string>(
            new AlphaOutRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
