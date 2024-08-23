using IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Alpha;
using IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Beta;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Handlers;

internal class AlphaHandler(IModule<BetaModule> betaModule) :
    IEventHandler<EventToSend>,
    IEventHandler<AlphaOutEvent>,
    IRequestHandler<RequestToSend>,
    IRequestHandler<AlphaOutRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response>
{
    private readonly IModule<BetaModule> _betaModule = betaModule;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _betaModule.Publish(eventData);
        return Task.CompletedTask;
    }

    public Task Handle(AlphaOutEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _betaModule.Publish<BetaInEvent>(new(eventData.Result));
        return Task.CompletedTask;
    }

    public async Task Handle(RequestToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _betaModule.Execute(requestData);
    }

    public async Task Handle(AlphaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _betaModule.Execute<BetaInRequest>(new(requestData.Result));
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return await _betaModule.Execute<RequestWithResponseToSend, string>(requestData);
    }

    public async Task<AlphaOutRequestWithResponse.Response> Handle(AlphaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return new((await _betaModule.Execute<BetaInRequestWithResponse, BetaInRequestWithResponse.Response>(new(requestData.Result))).Value);
    }
}
