using IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Beta;
using IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Gamma;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Handlers;

internal class BetaHandler(IModule<GammaModule> gammaModule) :
    IEventHandler<EventToSend>,
    IEventHandler<BetaOutEvent>,
    IRequestHandler<RequestToSend>,
    IRequestHandler<BetaOutRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response>
{
    private readonly IModule<GammaModule> _gammaModule = gammaModule;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _gammaModule.Publish(eventData);
        return Task.CompletedTask;
    }

    public Task Handle(BetaOutEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _gammaModule.Publish<GammaInEvent>(new(eventData.Result));
        return Task.CompletedTask;
    }

    public async Task Handle(RequestToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _gammaModule.Execute(requestData);
    }

    public async Task Handle(BetaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _gammaModule.Execute<GammaInRequest>(new(requestData.Result));
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return await _gammaModule.Execute<RequestWithResponseToSend, string>(requestData);
    }

    public async Task<BetaOutRequestWithResponse.Response> Handle(BetaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return new((await _gammaModule.Execute<GammaInRequestWithResponse, GammaInRequestWithResponse.Response>(new(requestData.Result))).Value);
    }
}
