using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Contract
{
    /// <summary>
    /// Lets you define logic for handling messages sent out by a module you want to use
    /// </summary>
    public interface IOutboxConsumer
    {
        #region Events
        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="action">A method to be invoked when the event is published</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleEvent<Event>(Action<Event> action)
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="function">A method to be invoked when the event is published</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleEvent<Event>(Func<Event, Task> function)
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="function">A method to be invoked when the event is published</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleEvent<Event>(Func<Event, CancellationToken, Task> function)
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="handler">A handler to be used when the event is published</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleEvent<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the event handler will be created each time the event is published
        /// </remarks>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the event is published</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleEvent<Event, Handler>()
            where Handler : class, IEventHandler<Event>
            where Event : class, IEvent;
        #endregion

        #region Commands
        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="action">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleCommand<Command>(Action<Command> action)
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="function">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleCommand<Command>(Func<Command, Task> function)
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="function">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleCommand<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="handler">A handler to be used when the command is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleCommand<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the command handler will be created each time the command is received
        /// </remarks>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the command is received</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleCommand<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand;

        /// <summary>
        /// Ignores the specified <b>command</b> message
        /// </summary>
        /// <typeparam name="Command">The type of the command being ignored</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer IgnoreCommand<Command>()
            where Command : class, ICommand;
        #endregion

        #region Queries
        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the query response</typeparam>
        /// <param name="function">A method to be invoked when the query is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleQuery<Query, Response>(Func<Query, Response> function)
            where Query : class, IQuery<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the query response</typeparam>
        /// <param name="function">A method to be invoked when the query is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleQuery<Query, Response>(Func<Query, Task<Response>> function)
            where Query : class, IQuery<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the query response</typeparam>
        /// <param name="function">A method to be invoked when the query is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleQuery<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
            where Query : class, IQuery<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the query response</typeparam>
        /// <param name="handler">A handler to be used when the query is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleQuery<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the query handler will be created each time the query is received
        /// </remarks>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the query response</typeparam>
        /// <typeparam name="Handler">A handler to be used when the query is received</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleQuery<Query, Response, Handler>()
            where Handler : class, IQueryHandler<Query, Response>
            where Query : class, IQuery<Response>;

        /// <summary>
        /// Ignores the specified <b>query</b> message and returns the provided default response
        /// </summary>
        /// <typeparam name="Query">The type of the query being ignored</typeparam>
        /// <typeparam name="Response">The type of the query response</typeparam>
        /// <param name="response">The default response to return when the query is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer IgnoreQuery<Query, Response>(Response response)
            where Query : class, IQuery<Response>;
        #endregion
    }
}
