using Tycho.Events;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.UsingGenericEvents.SUT.Handlers;

internal class GenericAppForwardedEventHandler<T>(TestWorkflow<GenericEventResult<T>> testWorkflow) : IEventHandler<GenericAppForwardedEvent<T>>
{
    private readonly TestWorkflow<GenericEventResult<T>> _testWorkflow = testWorkflow;

    public Task HandleAsync(EventContext<GenericAppForwardedEvent<T>> context, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(new GenericEventResult<T>("forwarded", context.Payload.Data));
        return Task.CompletedTask;
    }
}
