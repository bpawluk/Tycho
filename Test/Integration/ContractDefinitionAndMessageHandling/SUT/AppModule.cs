using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Test.Integration.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace Test.Integration.ContractDefinitionAndMessageHandling.SUT;

// Events
public record HandledByLambdaEvent() : IEvent;
public record HandledByAsyncLambdaEvent() : IEvent;
public record HandledByHandlerInstanceEvent() : IEvent;
public record HandledByHandlerTypeEvent() : IEvent;

// Commands
public record HandledByLambdaCommand() : ICommand;
public record HandledByAsyncLambdaCommand() : ICommand;
public record HandledByHandlerInstanceCommand() : ICommand;
public record HandledByHandlerTypeCommand() : ICommand;

// Queries
public record HandledByLambdaQuery() : IQuery<string>;
public record HandledByAsyncLambdaQuery() : IQuery<string>;
public record HandledByHandlerInstanceQuery() : IQuery<string>;
public record HandledByHandlerTypeQuery() : IQuery<string>;


internal class AppModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();
        module.SubscribesTo<HandledByLambdaEvent>(e => thisModule.PublishEvent(e))
              .SubscribesTo<HandledByAsyncLambdaEvent>((e, t) =>
              {
                  thisModule.PublishEvent(e, t);
                  return Task.CompletedTask;
              })
              .SubscribesTo(new HandledByHandlerInstanceEventHandler(thisModule))
              .SubscribesTo<HandledByHandlerTypeEvent, HandledByHandlerTypeEventHandler>();

        module.Executes<HandledByLambdaCommand>(c => { thisModule.ExecuteCommand(c); })
              .Executes<HandledByAsyncLambdaCommand>((c, t) => thisModule.ExecuteCommand(c, t))
              .Executes(new HandledByHandlerInstanceCommandHandler(thisModule))
              .Executes<HandledByHandlerTypeCommand, HandledByHandlerTypeCommandHandler>();

        module.RespondsTo<HandledByLambdaQuery, string>(q => thisModule.ExecuteQuery<HandledByLambdaQuery, string>(q).Result)
              .RespondsTo<HandledByAsyncLambdaQuery, string>(thisModule.ExecuteQuery<HandledByAsyncLambdaQuery, string>)
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

    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton(this);
    }
}
