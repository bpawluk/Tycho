using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Contract
{
    public interface IOutboxConsumer
    {
        #region Events
        IOutboxConsumer HandleEvent<Event>(Action<Event> action)
            where Event : class, IEvent;

        IOutboxConsumer HandleEvent<Event>(Func<Event, Task> function)
            where Event : class, IEvent;

        IOutboxConsumer HandleEvent<Event>(Func<Event, CancellationToken, Task> function)
            where Event : class, IEvent;

        IOutboxConsumer HandleEvent<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent;

        IOutboxConsumer HandleEvent<Event, Handler>()
            where Handler : class, IEventHandler<Event>
            where Event : class, IEvent;
        #endregion

        #region Commands
        IOutboxConsumer HandleCommand<Command>(Action<Command> action)
            where Command : class, ICommand;

        IOutboxConsumer HandleCommand<Command>(Func<Command, Task> function)
            where Command : class, ICommand;

        IOutboxConsumer HandleCommand<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand;

        IOutboxConsumer HandleCommand<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand;

        IOutboxConsumer HandleCommand<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand;

        IOutboxConsumer IgnoreCommand<Command>()
            where Command : class, ICommand;
        #endregion

        #region Queries
        IOutboxConsumer HandleQuery<Query, Response>(Func<Query, Response> function)
            where Query : class, IQuery<Response>;

        IOutboxConsumer HandleQuery<Query, Response>(Func<Query, Task<Response>> function)
            where Query : class, IQuery<Response>;

        IOutboxConsumer HandleQuery<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
            where Query : class, IQuery<Response>;

        IOutboxConsumer HandleQuery<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>;

        IOutboxConsumer HandleQuery<Query, Response, Handler>()
            where Handler : class, IQueryHandler<Query, Response>
            where Query : class, IQuery<Response>;

        IOutboxConsumer HandleQuery<Query, Response>(Response response)
            where Query : class, IQuery<Response>;
        #endregion
    }
}
