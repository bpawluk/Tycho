using TychoV2.Requests;
using TychoV2.Structure;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma.Handlers;

internal class GammaInRequestHandler(IParent parent)
    : IRequestHandler<GammaInRequest>
    , IRequestHandler<GammaInRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(GammaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.Execute(new GammaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(GammaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.Execute<GammaOutRequestWithResponse, string>(
            new GammaOutRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
