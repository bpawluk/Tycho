using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    internal class LambdaWrappingBase<Message, Result>
    {
        protected readonly Func<Message, CancellationToken, Result> _handle;

        public LambdaWrappingBase(Func<Message, CancellationToken, Result> handler)
        {
            _handle = handler;
        }

        protected static Func<Message, CancellationToken, Result> Wrap(Action<Message> handler, Result result)
        {
            return (message, _) =>
            {
                handler(message);
                return result;
            };
        }

        protected static Func<Message, CancellationToken, Result> Wrap(Func<Message, Result> handler)
        {
            return (message, _) => handler(message);
        }
    }

    internal class LambdaWrappingEventHandler<Event>
        : LambdaWrappingBase<Event, Task>
        , IEventHandler<Event>
        where Event : class, IEvent
    {
        public LambdaWrappingEventHandler(Action<Event> handler)
            : base(Wrap(handler, Task.CompletedTask)) { }

        public LambdaWrappingEventHandler(Func<Event, Task> handler)
            : base(Wrap(handler)) { }

        public LambdaWrappingEventHandler(Func<Event, CancellationToken, Task> handler)
            : base(handler) { }

        public Task Handle(Event eventData, CancellationToken cancellationToken)
        {
            return _handle(eventData, cancellationToken);
        }
    }

    internal class LambdaWrappingCommandHandler<Command>
        : LambdaWrappingBase<Command, Task>
        , ICommandHandler<Command>
        where Command : class, ICommand
    {
        public LambdaWrappingCommandHandler(Action<Command> handler)
            : base(Wrap(handler, Task.CompletedTask)) { }

        public LambdaWrappingCommandHandler(Func<Command, Task> handler)
            : base(Wrap(handler)) { }

        public LambdaWrappingCommandHandler(Func<Command, CancellationToken, Task> handler)
            : base(handler) { }

        public Task Handle(Command commandData, CancellationToken cancellationToken)
        {
            return _handle(commandData, cancellationToken);
        }
    }

    internal class LambdaWrappingQueryHandler<Query, Response>
        : LambdaWrappingBase<Query, Task<Response>>
        , IQueryHandler<Query, Response>
        where Query : class, IQuery<Response>
    {
        public LambdaWrappingQueryHandler(Func<Query, Response> handler)
            : base(Wrap(handler)) { }

        public LambdaWrappingQueryHandler(Func<Query, Task<Response>> handler)
            : base(Wrap(handler)) { }

        public LambdaWrappingQueryHandler(Func<Query, CancellationToken, Task<Response>> handler)
            : base(handler) { }

        public Task<Response> Handle(Query queryData, CancellationToken cancellationToken)
        {
            return _handle(queryData, cancellationToken);
        }

        private static Func<Query, CancellationToken, Task<Response>> Wrap(Func<Query, Response> handler)
        {
            return (message, _) => Task.FromResult(handler(message));
        }
    }
}
