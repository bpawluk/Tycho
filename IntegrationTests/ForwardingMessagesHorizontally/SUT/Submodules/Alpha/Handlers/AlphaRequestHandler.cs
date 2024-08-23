using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Alpha.Handlers;

internal class AlphaRequestHandler(IModule module) : 
    IRequestHandler<RequestToForward>,
    IRequestHandler<AlphaInRequest>,
    IRequestHandler<RequestWithResponseToForward, string>,
    IRequestHandler<AlphaInRequestWithResponse, AlphaInRequestWithResponse.Response>
{
    private readonly IModule _module = module;

    public async Task Handle(RequestToForward requestData, CancellationToken cancellationToken)
    {
        await _module.Execute(requestData, cancellationToken);
    }

    public async Task Handle(AlphaInRequest requestData, CancellationToken cancellationToken)
    {
        await _module.Execute<AlphaOutRequest>(new(requestData.Result), cancellationToken);
    }

    public async Task<string> Handle(RequestWithResponseToForward requestData, CancellationToken cancellationToken)
    {
        return await _module.Execute<RequestWithResponseToForward, string>(requestData, cancellationToken);
    }

    public async Task<AlphaInRequestWithResponse.Response> Handle(AlphaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        var result = await _module.Execute<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response>(
            new(requestData.Result), 
            cancellationToken);
        return new(result.Value);
    }
}
