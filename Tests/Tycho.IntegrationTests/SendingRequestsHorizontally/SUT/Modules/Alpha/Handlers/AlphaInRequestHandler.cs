using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaInRequestHandler(IAlphaModuleParent parent)
    : IRequestHandler<AlphaInRequest>
    , IRequestHandler<AlphaInRequestWithResponse, string>
{
    private readonly IAlphaModuleParent _parent = parent;

    public Task HandleAsync(AlphaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new AlphaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(AlphaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new AlphaOutRequestWithResponse(requestData.Result), cancellationToken);
    }
}
