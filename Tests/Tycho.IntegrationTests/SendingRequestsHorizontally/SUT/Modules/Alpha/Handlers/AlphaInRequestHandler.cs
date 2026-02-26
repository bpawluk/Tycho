using Tycho.Requests;
using Tycho.Structure.External;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaInRequestHandler(IParentReference parent)
    : IRequestHandler<AlphaInRequest>
    , IRequestHandler<AlphaInRequestWithResponse, string>
{
    private readonly IParentReference _parent = parent;

    public Task HandleAsync(AlphaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return Task.CompletedTask;
        //return _parent.ExecuteAsync(new AlphaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(AlphaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return Task.FromResult("Error");
        //return _parent.ExecuteAsync<AlphaOutRequestWithResponse, string>(
        //    new AlphaOutRequestWithResponse(requestData.Result),
        //    cancellationToken);
    }
}