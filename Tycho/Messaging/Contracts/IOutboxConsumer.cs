using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Contracts;

public interface IOutboxConsumer
{
    #region Events
    IOutboxConsumer OnEvent<Event>(Action<Event> action)
        where Event : class, IEvent;

    IOutboxConsumer OnEvent<Event>(Func<Event, Task> function)
        where Event : class, IEvent;

    IOutboxConsumer OnEvent<Event>(Func<Event, CancellationToken, Task> function)
        where Event : class, IEvent;

    IOutboxConsumer OnEvent<Event>(IEventHandler<Event> handler)
        where Event : class, IEvent;

    IOutboxConsumer OnEvent<Event>(Func<IEventHandler<Event>> handlerCreator)
        where Event : class, IEvent;
    #endregion

    #region Commands
    IOutboxConsumer OnCommand<Command>(Action<Command> action)
        where Command : class, ICommand;

    IOutboxConsumer OnCommand<Command>(Func<Command, Task> function)
        where Command : class, ICommand;

    IOutboxConsumer OnCommand<Command>(Func<Command, CancellationToken, Task> function)
        where Command : class, ICommand;

    IOutboxConsumer OnCommand<Command>(ICommandHandler<Command> handler)
        where Command : class, ICommand;

    IOutboxConsumer OnCommand<Command>(Func<ICommandHandler<Command>> handlerCreator)
        where Command : class, ICommand;

    IOutboxConsumer Ignore<Command>()
        where Command : class, ICommand;
    #endregion

    #region Queries
    IOutboxConsumer OnQuery<Query, Response>(Func<Query, Response> function)
        where Query : class, IQuery<Response>;

    IOutboxConsumer OnQuery<Query, Response>(Func<Query, Task<Response>> function)
        where Query : class, IQuery<Response>;

    IOutboxConsumer OnQuery<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
        where Query : class, IQuery<Response>;

    IOutboxConsumer OnQuery<Query, Response>(IQueryHandler<Query, Response> handler)
        where Query : class, IQuery<Response>;

    IOutboxConsumer OnQuery<Query, Response>(Func<IQueryHandler<Query, Response>> handlerCreator)
        where Query : class, IQuery<Response>;

    IOutboxConsumer Return<Query, Response>(Response response)
        where Query : class, IQuery<Response>;
    #endregion
}
