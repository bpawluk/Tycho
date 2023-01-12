using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Contracts
{
    public interface IInboxDefiner
    {
        #region Events
        IInboxDefiner SubscribesTo<Event>(Action<Event> action)
            where Event : class, IEvent;

        IInboxDefiner SubscribesTo<Event>(Func<Event, Task> function)
            where Event : class, IEvent;

        IInboxDefiner SubscribesTo<Event>(Func<Event, CancellationToken, Task> function)
            where Event : class, IEvent;

        IInboxDefiner SubscribesTo<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent;

        IInboxDefiner SubscribesTo<Event, Handler>()
            where Handler : class, IEventHandler<Event>
            where Event : class, IEvent;
        #endregion

        #region Commands
        IInboxDefiner Executes<Command>(Action<Command> action)
            where Command : class, ICommand;

        IInboxDefiner Executes<Command>(Func<Command, Task> function)
            where Command : class, ICommand;

        IInboxDefiner Executes<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand;

        IInboxDefiner Executes<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand;

        IInboxDefiner Executes<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand;
        #endregion

        #region Queries
        IInboxDefiner RespondsTo<Query, Response>(Func<Query, Response> function)
            where Query : class, IQuery<Response>;

        IInboxDefiner RespondsTo<Query, Response>(Func<Query, Task<Response>> function)
            where Query : class, IQuery<Response>;

        IInboxDefiner RespondsTo<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
            where Query : class, IQuery<Response>;

        IInboxDefiner RespondsTo<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>;

        IInboxDefiner RespondsTo<Query, Response, Handler>()
            where Handler : class, IQueryHandler<Query, Response>
            where Query : class, IQuery<Response>;
        #endregion
    }
}
