using IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Alpha;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Handlers;

internal class AppHandler(IModule<AlphaModule> alphaModule) :
    IEventHandler<EventToSend>,
    IEventHandler<EventToSendWithMapping>,
    IRequestHandler<RequestToSend>,
    IRequestHandler<RequestToSendWithMapping>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<RequestWithResponseToSendWithMapping, string>
{
    private readonly IModule<AlphaModule> _alphaModule = alphaModule;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _alphaModule.Publish(eventData);
        return Task.CompletedTask;
    }

    public Task Handle(EventToSendWithMapping eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _alphaModule.Publish<AlphaInEvent>(new(eventData.Result));
        return Task.CompletedTask;
    }

    public async Task Handle(RequestToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _alphaModule.Execute(requestData);
    }

    public async Task Handle(RequestToSendWithMapping requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _alphaModule.Execute<AlphaInRequest>(new(requestData.Result));
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return await _alphaModule.Execute<RequestWithResponseToSend, string>(requestData);
    }

    public async Task<string> Handle(RequestWithResponseToSendWithMapping requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return (await _alphaModule.Execute<AlphaInRequestWithResponse, AlphaInRequestWithResponse.Response>(new(requestData.Result))).Value;
    }
}
