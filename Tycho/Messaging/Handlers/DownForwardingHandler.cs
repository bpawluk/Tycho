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
            _target.Publish(_messageMapping(eventData), cancellationToken);
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
            return _target.Execute(_messageMapping(commandData), cancellationToken);
        }
    }

    internal class DownForwardingQueryHandler<QueryIn, ResponseIn, QueryOut, ResponseOut, Module> 
        : ForwardingHandlerBase<QueryIn, QueryOut>
        , IQueryHandler<QueryIn, ResponseIn>
        where QueryIn : class, IQuery<ResponseIn>
        where QueryOut : class, IQuery<ResponseOut>
        where Module : TychoModule
    {
        private readonly Func<ResponseOut, ResponseIn> _responseMapping;

        public DownForwardingQueryHandler(
            ISubmodule<Module> submodule, 
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping) 
            : base(submodule, queryMapping) 
        {
            _responseMapping = responseMapping;
        }

        public async Task<ResponseIn> Handle(QueryIn queryData, CancellationToken cancellationToken)
        {
            var response = await _target.Execute<QueryOut, ResponseOut>(_messageMapping(queryData), cancellationToken);
            return _responseMapping(response);
        }
    }
}
