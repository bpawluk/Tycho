using Tycho.Events;
using static Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.AlphaModule;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;

internal class WorkflowStartedEventHandler(IPublisher publisher) : IEventHandler<WorkflowStartedEvent>
{
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<WorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new WorkflowFinishedEvent(context.Payload.Result, nameof(AlphaModule)), cancellationToken);
    }
}
