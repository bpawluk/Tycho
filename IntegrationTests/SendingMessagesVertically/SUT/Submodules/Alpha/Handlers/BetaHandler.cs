using IntegrationTests.SendingMessagesVertically.SUT.Submodules.Beta;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesVertically.SUT.Submodules.Alpha.Handlers;

internal class BetaHandler(IModule module) :
    IEventHandler<EventToSend>,
    IEventHandler<BetaOutEvent>,
    IRequestHandler<RequestToSend>,
    IRequestHandler<BetaOutRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response>
{
    private readonly IModule _module = module;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken = default)
    {
        eventData.Result.HandlingCount++;
        _module.Publish(eventData);
        return Task.CompletedTask;
    }

    public Task Handle(BetaOutEvent eventData, CancellationToken cancellationToken = default)
    {
        eventData.Result.HandlingCount++;
        _module.Publish<AlphaOutEvent>(new(eventData.Result));
        return Task.CompletedTask;
    }

    public async Task Handle(RequestToSend requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        await _module.Execute(requestData);
    }

    public async Task Handle(BetaOutRequest requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        await _module.Execute<AlphaOutRequest>(new(requestData.Result));
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        return await _module.Execute<RequestWithResponseToSend, string>(requestData);
    }

    public async Task<BetaOutRequestWithResponse.Response> Handle(BetaOutRequestWithResponse requestData, CancellationToken cancellationToken = default)
    {
        requestData.Result.HandlingCount++;
        return new((await _module.Execute<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response>(new(requestData.Result))).Value);
    }
}
