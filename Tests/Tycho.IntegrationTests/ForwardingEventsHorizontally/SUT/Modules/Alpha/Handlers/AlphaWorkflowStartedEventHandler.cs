using Tycho.Events;
using Tycho.Events.Publishing;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaWorkflowStartedEventHandler(IGenericPublisher publisher) : IEventHandler<AlphaWorkflowStartedEvent>
{
    private readonly IGenericPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<AlphaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new AlphaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}