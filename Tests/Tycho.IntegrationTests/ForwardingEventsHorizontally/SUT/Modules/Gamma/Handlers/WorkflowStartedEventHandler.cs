using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma.Handlers;

internal class WorkflowStartedEventHandler(IEventPublisher publisher) : IEventHandler<WorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<WorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new WorkflowFinishedEvent(context.Payload.Result, typeof(GammaModule)), cancellationToken);
    }
}