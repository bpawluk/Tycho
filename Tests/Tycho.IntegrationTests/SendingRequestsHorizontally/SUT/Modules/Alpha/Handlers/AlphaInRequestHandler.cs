using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaInRequestHandler(IParent parent)
    : IRequestHandler<AlphaInRequest>
    , IRequestHandler<AlphaInRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(AlphaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new AlphaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(AlphaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync<AlphaOutRequestWithResponse, string>(
            new AlphaOutRequestWithResponse(requestData.Result),
            cancellationToken);
    }
}