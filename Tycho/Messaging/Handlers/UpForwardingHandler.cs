using System.Threading.Tasks;
using System.Threading;
using System;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    internal class UpForwardingEventHandler<EventIn, EventOut>
        : ForwardingHandlerBase<EventIn, EventOut>
        , IEventHandler<EventIn>
        where EventIn : class, IEvent
        where EventOut : class, IEvent
    {
        public UpForwardingEventHandler(IModule module, Func<EventIn, EventOut> mapping)
            : base(module, mapping) { }

        public Task Handle(EventIn eventData, CancellationToken cancellationToken)
        {
            _target.Publish(_mapping(eventData), cancellationToken);
            return Task.CompletedTask;
        }
    }

    internal class UpForwardingCommandHandler<CommandIn, CommandOut>
        : ForwardingHandlerBase<CommandIn, CommandOut>
        , ICommandHandler<CommandIn>
        where CommandIn : class, ICommand
        where CommandOut : class, ICommand
    {
        public UpForwardingCommandHandler(IModule module, Func<CommandIn, CommandOut> mapping)
            : base(module, mapping) { }

        public Task Handle(CommandIn commandData, CancellationToken cancellationToken)
        {
            return _target.Execute(_mapping(commandData), cancellationToken);
        }
    }

    internal class UpForwardingQueryHandler<QueryIn, QueryOut, Response>
        : ForwardingHandlerBase<QueryIn, QueryOut>
        , IQueryHandler<QueryIn, Response>
        where QueryIn : class, IQuery<Response>
        where QueryOut : class, IQuery<Response>
    {
        public UpForwardingQueryHandler(IModule module, Func<QueryIn, QueryOut> mapping) 
            : base(module, mapping) { }

        public Task<Response> Handle(QueryIn queryData, CancellationToken cancellationToken)
        {
            return _target.Execute<QueryOut, Response>(_mapping(queryData), cancellationToken);
        }
    }
}
