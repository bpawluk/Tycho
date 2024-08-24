using IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Gamma;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Handlers;

internal class GammaHandler(IModule module) :
    IEventHandler<EventToSend>,
    IEventHandler<GammaOutEvent>,
    IRequestHandler<RequestToSend>,
    IRequestHandler<GammaOutRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<GammaOutRequestWithResponse, GammaOutRequestWithResponse.Response>
{
    private readonly IModule _module = module;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _module.Publish(eventData);
        return Task.CompletedTask;
    }

    public Task Handle(GammaOutEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _module.Publish<MappedEvent>(new(eventData.Result));
        return Task.CompletedTask;
    }

    public async Task Handle(RequestToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _module.Execute(requestData);
    }

    public async Task Handle(GammaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _module.Execute<MappedRequest>(new(requestData.Result));
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return await _module.Execute<RequestWithResponseToSend, string>(requestData);
    }

    public async Task<GammaOutRequestWithResponse.Response> Handle(GammaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return new(await _module.Execute<MappedRequestWithResponse, string>(new(requestData.Result)));
    }
}
