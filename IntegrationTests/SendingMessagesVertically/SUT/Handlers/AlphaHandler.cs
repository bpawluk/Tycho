using IntegrationTests.SendingMessagesVertically.SUT.Submodules.Alpha;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesVertically.SUT.Handlers;

internal class AlphaHandler(IModule module) :
    IEventHandler<EventToSend>,
    IEventHandler<AlphaOutEvent>,
    IRequestHandler<RequestToSend>,
    IRequestHandler<AlphaOutRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response>
{
    private readonly IModule _module = module;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _module.Publish(eventData);
        return Task.CompletedTask;
    }

    public Task Handle(AlphaOutEvent eventData, CancellationToken cancellationToken)
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

    public async Task Handle(AlphaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        await _module.Execute<MappedRequest>(new(requestData.Result));
    }

    public async Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return await _module.Execute<RequestWithResponseToSend, string>(requestData);
    }

    public async Task<AlphaOutRequestWithResponse.Response> Handle(AlphaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return new(await _module.Execute<MappedRequestWithResponse, string>(new(requestData.Result)));
    }
}
