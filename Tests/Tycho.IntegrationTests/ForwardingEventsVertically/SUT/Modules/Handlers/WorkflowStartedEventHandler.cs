using Tycho.Events;
using static Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules.GammaModule;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules.Handlers;

internal class WorkflowStartedEventHandler(IPublisher publisher)
    : IEventHandler<WorkflowStartedEvent>
{
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<WorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new WorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}
