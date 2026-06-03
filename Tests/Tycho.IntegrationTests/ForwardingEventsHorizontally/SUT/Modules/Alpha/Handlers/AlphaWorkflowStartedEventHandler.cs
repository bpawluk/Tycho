using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaWorkflowStartedEventHandler(IAlphaModule.IPublisher publisher) : IEventHandler<AlphaWorkflowStartedEvent>
{
    private readonly IAlphaModule.IPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<AlphaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new AlphaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}
