using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;

internal class RequestHandler(IModule<AlphaModule> alphaModule)
    : IRequestHandler<Request>
    , IRequestHandler<RequestWithResponse, string>
{
    private readonly IModule<AlphaModule> _alphaModule = alphaModule;

    public Task Handle(Request requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _alphaModule.Execute(new AlphaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(RequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _alphaModule.Execute<AlphaInRequestWithResponse, string>(
            new AlphaInRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
