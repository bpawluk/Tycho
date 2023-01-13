using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Messaging.Contracts;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace SampleApp.App;

public sealed class AppModule : ModuleDefinition
{
    public record IncomingEvent() : IEvent;
    public record IncomingCommand() : ICommand;
    public record IncomingQuery() : IQuery<bool>;

    protected override void DeclareIncomingMessages(IInboxBuilder module, IServiceProvider services)
    {
        module.SubscribesTo((IncomingEvent eventData) => { })
              .Executes((IncomingCommand commandData) => { })
              .RespondsTo((IncomingQuery queryData) => false);
    }

    public record OutgoingEvent() : IEvent;
    public record OutgoingCommand() : ICommand;
    public record OutgoingQuery() : IQuery<bool>;

    protected override void DeclareOutgoingMessages(IOutboxProducer module, IServiceProvider services)
    {
        module.Publishes<OutgoingEvent>()
              .Sends<OutgoingCommand>()
              .Sends<OutgoingQuery, bool>();
    }

    protected override void IncludeSubmodules(ISubmodulesDefiner submodules, IServiceProvider services)
    {
        submodules.Add<AppModule>((IOutboxConsumer submodule) =>
        {
            submodule.OnEvent((OutgoingEvent eventData) => { })
                     .OnCommand((OutgoingCommand commandData) => { })
                     .OnQuery((OutgoingQuery queryData) => false);
        });
    }

    protected override void RegisterServices(IServiceCollection services) 
    { 

    }
}
