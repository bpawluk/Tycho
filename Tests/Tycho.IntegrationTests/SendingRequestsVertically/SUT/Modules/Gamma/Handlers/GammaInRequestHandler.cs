using Tycho.Requests;
using static Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma.GammaModule;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma.Handlers;

internal class GammaInRequestHandler(IParent parent)
    : IRequestHandler<GammaInRequest>
    , IRequestHandler<GammaInRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(GammaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new GammaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(GammaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new GammaOutRequestWithResponse(requestData.Result), cancellationToken);
    }
}