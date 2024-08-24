using IntegrationTests.SendingMessagesVertically.SUT.Submodules.Gamma;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesVertically.SUT.Submodules.Beta.Handlers;

internal class BetaHandler(IModule<GammaModule> gammaModule) :
    IEventHandler<EventToSend>,
    IEventHandler<BetaInEvent>,
    IRequestHandler<RequestToSend>,
    IRequestHandler<BetaInRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<BetaInRequestWithResponse, BetaInRequestWithResponse.Response>
{
    private readonly IModule<GammaModule> _gammaModule = gammaModule;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken = default)
    {
        eventData.Result.HandlingCount++;
        _gammaModule.Publish(eventData);
        return Task.CompletedTask;
    }

    public Task Handle(BetaInEvent eventData, CancellationToken cancellationToken = default)
    {
        eventData.Result.HandlingCount++;
        _gammaModule.Publish<GammaInEvent>(new(eventData.Result));
        return Task.CompletedTask;
    }

    public async Task Handle(RequestToSend requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        await _gammaModule.Execute(requestData);
    }

    public async Task Handle(BetaInRequest requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        await _gammaModule.Execute<GammaInRequest>(new(requestData.Result));
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        return await _gammaModule.Execute<RequestWithResponseToSend, string>(requestData);
    }

    public async Task<BetaInRequestWithResponse.Response> Handle(BetaInRequestWithResponse requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        return new((await _gammaModule.Execute<GammaInRequestWithResponse, GammaInRequestWithResponse.Response>(new(requestData.Result))).Value);
    }
}
