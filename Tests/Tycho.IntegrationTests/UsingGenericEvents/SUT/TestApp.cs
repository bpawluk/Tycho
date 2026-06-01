using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.UsingGenericEvents.SUT.Handlers;
using Tycho.IntegrationTests.UsingGenericEvents.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericEvents.SUT;

public record GenericEventResult<T>(string Path, T Data);

public record PublishGenericAppIntEventRequest(int Data) : IRequest;

public record PublishGenericAppStringEventRequest(string Data) : IRequest;

public record PublishGenericForwardedIntEventRequest(int Data) : IRequest;

public record PublishGenericForwardedStringEventRequest(string Data) : IRequest;

public record GenericAppEvent<T>(T Data) : IEvent;

public record GenericAppEventToForward<T>(T Data) : IEvent;

public record GenericAppForwardedEvent<T>(T Data) : IEvent;

[TychoDefinition]
public partial class TestApp(
    TestWorkflow<GenericEventResult<int>> intWorkflow,
    TestWorkflow<GenericEventResult<string>> stringWorkflow)
    : TychoApp
{
    private readonly TestWorkflow<GenericEventResult<int>> _intWorkflow = intWorkflow;
    private readonly TestWorkflow<GenericEventResult<string>> _stringWorkflow = stringWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Handles<PublishGenericAppIntEventRequest, GenericEventWorkflowRequestHandler>();
        app.Handles<PublishGenericAppStringEventRequest, GenericEventWorkflowRequestHandler>();
        app.Handles<PublishGenericForwardedIntEventRequest, GenericEventWorkflowRequestHandler>();
        app.Handles<PublishGenericForwardedStringEventRequest, GenericEventWorkflowRequestHandler>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Handles<GenericAppEvent<int>, GenericAppEventHandler<int>>();
        app.Handles<GenericAppEvent<string>, GenericAppEventHandler<string>>();

        app.Routes<GenericAppEventToForward<int>>()
           .ForwardsAs<GenericModuleEvent<int>, TestModule>(
               eventData => new GenericModuleEvent<int>(eventData.Data));

        app.Routes<GenericAppEventToForward<string>>()
           .ForwardsAs<GenericModuleEvent<string>, TestModule>(
               eventData => new GenericModuleEvent<string>(eventData.Data));

        app.Handles<GenericAppForwardedEvent<int>, GenericAppForwardedEventHandler<int>>();
        app.Handles<GenericAppForwardedEvent<string>, GenericAppForwardedEventHandler<string>>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<TestModule>();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_intWorkflow);
        app.AddSingleton(_stringWorkflow);
    }
}
