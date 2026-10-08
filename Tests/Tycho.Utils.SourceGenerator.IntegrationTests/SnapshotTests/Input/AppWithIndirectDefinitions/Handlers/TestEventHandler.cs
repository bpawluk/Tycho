using Tycho.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Handlers;

internal class TestEventHandler
    : IEventHandler<TestEventFromLocalHelper>
    , IEventHandler<TestEventFromLocalStaticHelper>
    , IEventHandler<TestEventFromHelperClass>
    , IEventHandler<TestEventFromHelperStaticClass>
    , IEventHandler<TestEventFromHelperExtension>
{
    public Task HandleAsync(EventContext<TestEventFromLocalHelper> context, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task HandleAsync(EventContext<TestEventFromLocalStaticHelper> context, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task HandleAsync(EventContext<TestEventFromHelperClass> context, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task HandleAsync(EventContext<TestEventFromHelperStaticClass> context, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task HandleAsync(EventContext<TestEventFromHelperExtension> context, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
