using Tycho.Events;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.IntegrationTests.SUT.Handlers;

internal class BeginTestWorkflowRequestHandler(IEventPublisher publisher) : IRequestHandler<BeginTestWorkflowRequest>
{
    private readonly IEventPublisher _publisher = publisher;

    public Task Handle(BeginTestWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        return _publisher.Publish(new TestEvent(requestData.Result), cancellationToken);
    }
}