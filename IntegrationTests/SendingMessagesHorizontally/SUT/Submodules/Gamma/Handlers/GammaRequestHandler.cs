using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Gamma.Handlers;

internal class GammaRequestHandler(IModule module) : 
    IRequestHandler<RequestToSend>,
    IRequestHandler<GammaInRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<GammaInRequestWithResponse, GammaInRequestWithResponse.Response>
{
    private readonly IModule _module = module;

    public async Task Handle(RequestToSend requestData, CancellationToken cancellationToken)
    {
        await _module.Execute(requestData, cancellationToken);
    }

    public async Task Handle(GammaInRequest requestData, CancellationToken cancellationToken)
    {
        await _module.Execute<GammaOutRequest>(new(requestData.Result), cancellationToken);
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken)
    {
        return await _module.Execute<RequestWithResponseToSend, string>(requestData, cancellationToken);
    }

    public async Task<GammaInRequestWithResponse.Response> Handle(GammaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        var result = await _module.Execute<GammaOutRequestWithResponse, GammaOutRequestWithResponse.Response>(
            new(requestData.Result), 
            cancellationToken);
        return new(result.Value);
    }
}
