using Tycho.Events;
using static Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.AlphaModule;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaWorkflowStartedEventHandler(IPublisher publisher) : IEventHandler<AlphaWorkflowStartedEvent>
{
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<AlphaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new AlphaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}