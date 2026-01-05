using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha;
using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Handlers;

internal class RequestHandler(IModule<AlphaModule> alphaModule)
    : IRequestHandler<Request>
    , IRequestHandler<RequestWithResponse, string>
{
    private readonly IModule<AlphaModule> _alphaModule = alphaModule;

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