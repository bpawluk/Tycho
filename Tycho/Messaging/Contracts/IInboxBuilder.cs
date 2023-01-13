using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Contracts
{
    public interface IInboxBuilder
    {
        #region Events
        IInboxBuilder SubscribesTo<Event>(Action<Event> action)
            where Event : class, IEvent;

        IInboxBuilder SubscribesTo<Event>(Func<Event, Task> function)
            where Event : class, IEvent;

        IInboxBuilder SubscribesTo<Event>(Func<Event, CancellationToken, Task> function)
            where Event : class, IEvent;

        IInboxBuilder SubscribesTo<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent;

        IInboxBuilder SubscribesTo<Event, Handler>()
            where Handler : class, IEventHandler<Event>
            where Event : class, IEvent;
        #endregion

        #region Commands
        IInboxBuilder Executes<Command>(Action<Command> action)
            where Command : class, ICommand;

        IInboxBuilder Executes<Command>(Func<Command, Task> function)
            where Command : class, ICommand;

        IInboxBuilder Executes<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand;

        IInboxBuilder Executes<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand;

        IInboxBuilder Executes<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand;
        #endregion

        #region Queries
        IInboxBuilder RespondsTo<Query, Response>(Func<Query, Response> function)
            where Query : class, IQuery<Response>;

        IInboxBuilder RespondsTo<Query, Response>(Func<Query, Task<Response>> function)
            where Query : class, IQuery<Response>;

        IInboxBuilder RespondsTo<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
            where Query : class, IQuery<Response>;

        IInboxBuilder RespondsTo<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>;

        IInboxBuilder RespondsTo<Query, Response, Handler>()
            where Handler : class, IQueryHandler<Query, Response>
            where Query : class, IQuery<Response>;
        #endregion
    }
}
