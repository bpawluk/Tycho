using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.IntegrationTests.UsingGenericEvents.SUT.Handlers;
using Tycho.IntegrationTests.UsingGenericEvents.SUT.Modules;
using Tycho.IntegrationTests._Utils;
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
public class TestApp(
    TestWorkflow<GenericEventResult<int>> intWorkflow,
    TestWorkflow<GenericEventResult<string>> stringWorkflow)
    : TychoApp
{
    private readonly TestWorkflow<GenericEventResult<int>> _intWorkflow = intWorkflow;
    private readonly TestWorkflow<GenericEventResult<string>> _stringWorkflow = stringWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Expects<PublishGenericAppIntEventRequest>()
           .HandlesWith<GenericEventWorkflowRequestHandler>();

        app.Expects<PublishGenericAppStringEventRequest>()
           .HandlesWith<GenericEventWorkflowRequestHandler>();

        app.Expects<PublishGenericForwardedIntEventRequest>()
           .HandlesWith<GenericEventWorkflowRequestHandler>();

        app.Expects<PublishGenericForwardedStringEventRequest>()
           .HandlesWith<GenericEventWorkflowRequestHandler>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Expects<GenericAppEvent<int>>()
           .HandlesWith<GenericAppEventHandler<int>>();

        app.Expects<GenericAppEvent<string>>()
           .HandlesWith<GenericAppEventHandler<string>>();

        app.Expects<GenericAppEventToForward<int>>()
           .MapsTo<GenericModuleEvent<int>>(payload => new(payload.Data))
           .ForwardsTo<TestModule>();

        app.Expects<GenericAppEventToForward<string>>()
           .MapsTo<GenericModuleEvent<string>>(payload => new(payload.Data))
           .ForwardsTo<TestModule>();

        app.Expects<GenericAppForwardedEvent<int>>()
           .HandlesWith<GenericAppForwardedEventHandler<int>>();

        app.Expects<GenericAppForwardedEvent<string>>()
           .HandlesWith<GenericAppForwardedEventHandler<string>>();
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
