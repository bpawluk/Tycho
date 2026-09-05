using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericEvents.SUT.Handlers;

internal class GenericEventWorkflowRequestHandler(ITestAppPublisher publisher)
    : IRequestHandler<PublishGenericAppIntEventRequest>
    , IRequestHandler<PublishGenericAppStringEventRequest>
    , IRequestHandler<PublishGenericForwardedIntEventRequest>
    , IRequestHandler<PublishGenericForwardedStringEventRequest>
{
    private readonly ITestAppPublisher _publisher = publisher;

    public async Task HandleAsync(PublishGenericAppIntEventRequest requestData, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new GenericAppEvent<int>(requestData.Data), cancellationToken);
    }

    public async Task HandleAsync(PublishGenericAppStringEventRequest requestData, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new GenericAppEvent<string>(requestData.Data), cancellationToken);
    }

    public async Task HandleAsync(PublishGenericForwardedIntEventRequest requestData, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new GenericAppEventToForward<int>(requestData.Data), cancellationToken);
    }

    public async Task HandleAsync(PublishGenericForwardedStringEventRequest requestData, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new GenericAppEventToForward<string>(requestData.Data), cancellationToken);
    }
}
