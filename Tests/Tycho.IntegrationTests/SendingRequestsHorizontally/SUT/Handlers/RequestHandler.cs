using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha;
using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;

internal class RequestHandler(IModuleInstance<AlphaModule> alphaModule)
    : IRequestHandler<Request>
    , IRequestHandler<RequestWithResponse, string>
{
    private readonly IModuleInstance<AlphaModule> _alphaModule = alphaModule;

    public Task HandleAsync(Request requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _alphaModule.ExecuteAsync(new AlphaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(RequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _alphaModule.ExecuteAsync<AlphaInRequestWithResponse, string>(
            new AlphaInRequestWithResponse(requestData.Result),
            cancellationToken);
    }
}