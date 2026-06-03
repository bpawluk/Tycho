using Tycho.Events;

namespace Tycho.IntegrationTests.UsingGenericEvents.SUT.Modules.Handlers;

internal class GenericModuleEventHandler<T>(ITestModule.IPublisher publisher) : IEventHandler<GenericModuleEvent<T>>
{
    public async Task HandleAsync(EventContext<GenericModuleEvent<T>> context, CancellationToken cancellationToken)
    {
        if (context.Payload is GenericModuleEvent<string> stringPayload)
        {
            await publisher.PublishAsync(new GenericModuleFinishedEvent<string>(stringPayload.Data), cancellationToken);

        }

        if (context.Payload is GenericModuleEvent<int> intPayload)
        {
            await publisher.PublishAsync(new GenericModuleFinishedEvent<int>(intPayload.Data), cancellationToken);

        }

        throw new InvalidOperationException($"Unsupported request type: {typeof(T)}");
    }
}
