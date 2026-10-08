using Tycho.Events;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.UsingGenericEvents.SUT.Handlers;

internal class GenericAppEventHandler<T>(TestWorkflow<GenericEventResult<T>> testWorkflow) : IEventHandler<GenericAppEvent<T>>
{
    private readonly TestWorkflow<GenericEventResult<T>> _testWorkflow = testWorkflow;

    public Task HandleAsync(EventContext<GenericAppEvent<T>> context, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(new GenericEventResult<T>("app", context.Payload.Data));
        return Task.CompletedTask;
    }
}
