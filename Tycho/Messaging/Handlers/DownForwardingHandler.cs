using System.Threading.Tasks;
using System.Threading;
using Tycho.Messaging.Payload;
using System;

namespace Tycho.Messaging.Handlers
{
    internal class DownForwardingEventHandler<EventIn, EventOut, Module> 
        : ForwardingHandlerBase<EventIn, EventOut>
        , IEventHandler<EventIn>
        where EventIn: class, IEvent
        where EventOut: class, IEvent
        where Module : TychoModule
    {
        public DownForwardingEventHandler(ISubmodule<Module> submodule, Func<EventIn, EventOut> mapping) 
            : base(submodule, mapping) { }

        public Task Handle(EventIn eventData, CancellationToken cancellationToken)
        {
            _target.Publish(_mapping(eventData), cancellationToken);
            return Task.CompletedTask;
        }
    }

    internal class DownForwardingCommandHandler<CommandIn, CommandOut, Module> 
        : ForwardingHandlerBase<CommandIn, CommandOut>
        , ICommandHandler<CommandIn>
        where CommandIn : class, ICommand
        where CommandOut : class, ICommand
        where Module : TychoModule
    {
        public DownForwardingCommandHandler(ISubmodule<Module> submodule, Func<CommandIn, CommandOut> mapping) 
            : base(submodule, mapping) { }

        public Task Handle(CommandIn commandData, CancellationToken cancellationToken)
        {
            return _target.Execute(_mapping(commandData), cancellationToken);
        }
    }

    internal class DownForwardingQueryHandler<QueryIn, QueryOut, Response, Module> 
        : ForwardingHandlerBase<QueryIn, QueryOut>
        , IQueryHandler<QueryIn, Response>
        where QueryIn : class, IQuery<Response>
        where QueryOut : class, IQuery<Response>
        where Module : TychoModule
    {
        public DownForwardingQueryHandler(ISubmodule<Module> submodule, Func<QueryIn, QueryOut> mapping) 
            : base(submodule, mapping) { }

        public Task<Response> Handle(QueryIn queryData, CancellationToken cancellationToken)
        {
            return _target.Execute<QueryOut, Response>(_mapping(queryData), cancellationToken);
        }
    }
}
