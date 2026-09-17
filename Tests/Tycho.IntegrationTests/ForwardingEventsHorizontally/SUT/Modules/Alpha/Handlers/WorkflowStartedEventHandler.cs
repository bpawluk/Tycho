using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;

internal class WorkflowStartedEventHandler(IAlphaModulePublisher publisher) : IEventHandler<WorkflowStartedEvent>
{
    private readonly IAlphaModulePublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<WorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new WorkflowFinishedEvent(context.Payload.Result, nameof(AlphaModule)), cancellationToken);
    }
}
