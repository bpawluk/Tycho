using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Beta.Handlers;

internal class BetaRequestHandler(IModule module) : 
    IRequestHandler<RequestToSend>,
    IRequestHandler<BetaInRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<BetaInRequestWithResponse, BetaInRequestWithResponse.Response>
{
    private readonly IModule _module = module;

    public async Task Handle(RequestToSend requestData, CancellationToken cancellationToken)
    {
        await _module.Execute(requestData, cancellationToken);
    }

    public async Task Handle(BetaInRequest requestData, CancellationToken cancellationToken)
    {
        await _module.Execute<BetaOutRequest>(new(requestData.Result), cancellationToken);
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken)
    {
        return await _module.Execute<RequestWithResponseToSend, string>(requestData, cancellationToken);
    }

    public async Task<BetaInRequestWithResponse.Response> Handle(BetaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        var result = await _module.Execute<BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response>(
            new(requestData.Result), 
            cancellationToken);
        return new(result.Value);
    }
}
