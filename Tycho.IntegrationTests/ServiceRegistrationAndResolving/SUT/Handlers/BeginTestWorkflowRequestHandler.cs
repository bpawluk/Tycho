using Tycho.Events;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class BeginTestWorkflowRequestHandler(IEventPublisher publisher) : IRequestHandler<BeginTestWorkflowRequest>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(BeginTestWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        if (requestData.Result.Id == "event-app-singleton-workflow")
        {
            await _publisher.Publish(new GetAppSingletonServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "event-app-scoped-workflow")
        {
            await _publisher.Publish(new GetAppScopedServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "event-app-transient-workflow")
        {
            await _publisher.Publish(new GetAppTransientServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "event-module-singleton-workflow")
        {
            await _publisher.Publish(new GetModuleSingletonServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "event-module-scoped-workflow")
        {
            await _publisher.Publish(new GetModuleScopedServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "event-module-transient-workflow")
        {
            await _publisher.Publish(new GetModuleTransientServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else
        {
            throw new ArgumentException($"Unknown workflow ID {requestData.Result.Id}");
        }
    }
}