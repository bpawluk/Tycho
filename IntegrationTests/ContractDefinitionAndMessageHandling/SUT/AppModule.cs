using IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT;

// Events
public record HandledByLambdaEvent() : IEvent;
public record HandledByAsyncLambdaEvent() : IEvent;
public record HandledByHandlerInstanceEvent() : IEvent;
public record HandledByHandlerTypeEvent() : IEvent;

// Commands
public record HandledByLambdaCommand() : IRequest;
public record HandledByAsyncLambdaCommand() : IRequest;
public record HandledByHandlerInstanceCommand() : IRequest;
public record HandledByHandlerTypeCommand() : IRequest;

// Queries
public record HandledByLambdaQuery() : IRequest<string>;
public record HandledByAsyncLambdaQuery() : IRequest<string>;
public record HandledByHandlerInstanceQuery() : IRequest<string>;
public record HandledByHandlerTypeQuery() : IRequest<string>;


internal class AppModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();
        module.Events.Handle<HandledByLambdaEvent>(e => thisModule.Publish(e))
              .Events.Handle<HandledByAsyncLambdaEvent>((e, t) =>
              {
                  thisModule.Publish(e, t);
                  return Task.CompletedTask;
              })
              .Events.Handle(new HandledByHandlerInstanceEventHandler(thisModule))
              .Events.Handle<HandledByHandlerTypeEvent, HandledByHandlerTypeEventHandler>();

        module.Requests.Handle<HandledByLambdaCommand>(c => { thisModule.Execute(c); })
              .Requests.Handle<HandledByAsyncLambdaCommand>((c, t) => thisModule.Execute(c, t))
              .Requests.Handle(new HandledByHandlerInstanceCommandHandler(thisModule))
              .Requests.Handle<HandledByHandlerTypeCommand, HandledByHandlerTypeCommandHandler>();

        module.Requests.Handle<HandledByLambdaQuery, string>(q => thisModule.Execute<HandledByLambdaQuery, string>(q).Result)
              .Requests.Handle<HandledByAsyncLambdaQuery, string>(thisModule.Execute<HandledByAsyncLambdaQuery, string>)
              .Requests.Handle(new HandledByHandlerInstanceQueryHandler(thisModule))
              .Requests.Handle<HandledByHandlerTypeQuery, string, HandledByHandlerTypeQueryHandler>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<HandledByLambdaEvent>()
              .Events.Declare<HandledByAsyncLambdaEvent>()
              .Events.Declare<HandledByHandlerInstanceEvent>()
              .Events.Declare<HandledByHandlerTypeEvent>();

        module.Requests.Declare<HandledByLambdaCommand>()
              .Requests.Declare<HandledByAsyncLambdaCommand>()
              .Requests.Declare<HandledByHandlerInstanceCommand>()
              .Requests.Declare<HandledByHandlerTypeCommand>();

        module.Requests.Declare<HandledByLambdaQuery, string>()
              .Requests.Declare<HandledByAsyncLambdaQuery, string>()
              .Requests.Declare<HandledByHandlerInstanceQuery, string>()
              .Requests.Declare<HandledByHandlerTypeQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(this);
    }
}
