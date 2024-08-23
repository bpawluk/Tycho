using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Alpha.Handlers;

internal class AlphaRequestHandler(IModule module) : 
    IRequestHandler<RequestToSend>,
    IRequestHandler<AlphaInRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<AlphaInRequestWithResponse, AlphaInRequestWithResponse.Response>
{
    private readonly IModule _module = module;

    public async Task Handle(RequestToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _module.Execute(requestData, cancellationToken);
    }

    public async Task Handle(AlphaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _module.Execute<AlphaOutRequest>(new(requestData.Result), cancellationToken);
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return await _module.Execute<RequestWithResponseToSend, string>(requestData, cancellationToken);
    }

    public async Task<AlphaInRequestWithResponse.Response> Handle(AlphaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        var result = await _module.Execute<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response>(
            new(requestData.Result), 
            cancellationToken);
        return new(result.Value);
    }
}
