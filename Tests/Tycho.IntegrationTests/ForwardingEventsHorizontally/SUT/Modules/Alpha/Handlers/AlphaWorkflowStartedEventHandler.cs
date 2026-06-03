using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaWorkflowStartedEventHandler(IAlphaModulePublisher publisher) : IEventHandler<AlphaWorkflowStartedEvent>
{
    private readonly IAlphaModulePublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<AlphaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new AlphaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}
