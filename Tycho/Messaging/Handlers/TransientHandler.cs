using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    internal abstract class TransientBase<Handler>
    {
        protected readonly Func<Handler> _createHandler;

        public TransientBase(Func<Handler> handlerCreator)
        {
            _createHandler = handlerCreator;
        }
    }

    internal class TransientEventHandler<Event>
        : TransientBase<IEventHandler<Event>>
        , IEventHandler<Event>
        where Event : class, IEvent
    {
        public TransientEventHandler(Func<IEventHandler<Event>> handlerCreator)
            : base(handlerCreator) { }

        public Task Handle(Event eventData, CancellationToken cancellationToken)
        {
            var handler = _createHandler();
            return handler.Handle(eventData, cancellationToken);
        }
    }

    internal class TransientCommandHandler<Command>
        : TransientBase<ICommandHandler<Command>>
        , ICommandHandler<Command>
        where Command : class, ICommand
    {
        public TransientCommandHandler(Func<ICommandHandler<Command>> handlerCreator)
            : base(handlerCreator) { }

        public Task Handle(Command commandData, CancellationToken cancellationToken)
        {
            var handler = _createHandler();
            return handler.Handle(commandData, cancellationToken);
        }
    }

    internal class TransientQueryHandler<Query, Response>
        : TransientBase<IQueryHandler<Query, Response>>
        , IQueryHandler<Query, Response>
        where Query : class, IQuery<Response>
    {
        public TransientQueryHandler(Func<IQueryHandler<Query, Response>> handlerCreator)
            : base(handlerCreator) { }

        public Task<Response> Handle(Query queryData, CancellationToken cancellationToken)
        {
            var handler = _createHandler();
            return handler.Handle(queryData, cancellationToken);
        }
    }
}
