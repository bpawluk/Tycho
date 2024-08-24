using IntegrationTests.SendingMessagesVertically.SUT.Submodules.Beta;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesVertically.SUT.Submodules.Alpha.Handlers;

internal class AlphaHandler(IModule<BetaModule> betaModule) :
    IEventHandler<EventToSend>,
    IEventHandler<AlphaInEvent>,
    IRequestHandler<RequestToSend>,
    IRequestHandler<AlphaInRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<AlphaInRequestWithResponse, AlphaInRequestWithResponse.Response>
{
    private readonly IModule<BetaModule> _module = betaModule;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken = default)
    {
        eventData.Result.HandlingCount++;
        _module.Publish(eventData);
        return Task.CompletedTask;
    }

    public Task Handle(AlphaInEvent eventData, CancellationToken cancellationToken = default)
    {
        eventData.Result.HandlingCount++;
        _module.Publish<BetaInEvent>(new(eventData.Result));
        return Task.CompletedTask;
    }

    public async Task Handle(RequestToSend requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        await _module.Execute(requestData);
    }

    public async Task Handle(AlphaInRequest requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        await _module.Execute<BetaInRequest>(new(requestData.Result));
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        return await _module.Execute<RequestWithResponseToSend, string>(requestData);
    }

    public async Task<AlphaInRequestWithResponse.Response> Handle(AlphaInRequestWithResponse requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        return new((await _module.Execute<BetaInRequestWithResponse, BetaInRequestWithResponse.Response>(new(requestData.Result))).Value);
    }
}
