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
        module.SubscribesTo<HandledByLambdaEvent>(e => thisModule.Publish(e))
              .SubscribesTo<HandledByAsyncLambdaEvent>((e, t) =>
              {
                  thisModule.Publish(e, t);
                  return Task.CompletedTask;
              })
              .SubscribesTo(new HandledByHandlerInstanceEventHandler(thisModule))
              .SubscribesTo<HandledByHandlerTypeEvent, HandledByHandlerTypeEventHandler>();

        module.Executes<HandledByLambdaCommand>(c => { thisModule.Execute(c); })
              .Executes<HandledByAsyncLambdaCommand>((c, t) => thisModule.Execute(c, t))
              .Executes(new HandledByHandlerInstanceCommandHandler(thisModule))
              .Executes<HandledByHandlerTypeCommand, HandledByHandlerTypeCommandHandler>();

        module.RespondsTo<HandledByLambdaQuery, string>(q => thisModule.Execute<HandledByLambdaQuery, string>(q).Result)
              .RespondsTo<HandledByAsyncLambdaQuery, string>(thisModule.Execute<HandledByAsyncLambdaQuery, string>)
              .RespondsTo(new HandledByHandlerInstanceQueryHandler(thisModule))
              .RespondsTo<HandledByHandlerTypeQuery, string, HandledByHandlerTypeQueryHandler>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<HandledByLambdaEvent>()
              .Publishes<HandledByAsyncLambdaEvent>()
              .Publishes<HandledByHandlerInstanceEvent>()
              .Publishes<HandledByHandlerTypeEvent>();

        module.Sends<HandledByLambdaCommand>()
              .Sends<HandledByAsyncLambdaCommand>()
              .Sends<HandledByHandlerInstanceCommand>()
              .Sends<HandledByHandlerTypeCommand>();

        module.Sends<HandledByLambdaQuery, string>()
              .Sends<HandledByAsyncLambdaQuery, string>()
              .Sends<HandledByHandlerInstanceQuery, string>()
              .Sends<HandledByHandlerTypeQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(this);
    }
}
