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
            _target.Publish(_messageMapping(eventData), cancellationToken);
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
            return _target.Execute(_messageMapping(commandData), cancellationToken);
        }
    }

    internal class UpForwardingQueryHandler<QueryIn, ResponseIn, QueryOut, ResponseOut>
        : ForwardingHandlerBase<QueryIn, QueryOut>
        , IQueryHandler<QueryIn, ResponseIn>
        where QueryIn : class, IQuery<ResponseIn>
        where QueryOut : class, IQuery<ResponseOut>
    {
        private readonly Func<ResponseOut, ResponseIn> _responseMapping;

        public UpForwardingQueryHandler(
            IModule submodule,
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            : base(submodule, queryMapping)
        {
            _responseMapping = responseMapping;
        }

        public async Task<ResponseIn> Handle(QueryIn queryData, CancellationToken cancellationToken)
        {
            var result = await _target.Execute<QueryOut, ResponseOut>(_messageMapping(queryData), cancellationToken);
            return _responseMapping(result);
        }
    }
}
