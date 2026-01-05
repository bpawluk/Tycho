using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma.Handlers;

internal class GammaWorkflowStartedEventHandler(IEventPublisher publisher) : IEventHandler<GammaWorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<GammaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new GammaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}