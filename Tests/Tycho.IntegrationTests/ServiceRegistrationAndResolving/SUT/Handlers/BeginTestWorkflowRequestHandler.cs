using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.Requests;
using static Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.TestApp;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class BeginTestWorkflowRequestHandler(IPublisher publisher) : IRequestHandler<BeginTestWorkflowRequest>
{
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(BeginTestWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        if (requestData.Result.Id == "event-app-singleton-workflow")
        {
            await _publisher.PublishAsync(new GetAppSingletonServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "event-app-scoped-workflow")
        {
            await _publisher.PublishAsync(new GetAppScopedServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "event-app-transient-workflow")
        {
            await _publisher.PublishAsync(new GetAppTransientServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "event-module-singleton-workflow")
        {
            await _publisher.PublishAsync(new GetModuleSingletonServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "event-module-scoped-workflow")
        {
            await _publisher.PublishAsync(new GetModuleScopedServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "event-module-transient-workflow")
        {
            await _publisher.PublishAsync(new GetModuleTransientServiceUsageEvent(requestData.Result), cancellationToken);
        }
        else
        {
            throw new ArgumentException($"Unknown workflow ID {requestData.Result.Id}");
        }
    }
}