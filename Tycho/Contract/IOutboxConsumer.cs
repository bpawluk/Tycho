using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Contract
{
    /// <summary>
    /// Lets you define logic for handling messages sent out by a module that you want to use
    /// </summary>
    public interface IOutboxConsumer
    {
        #region Events
        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by passing it on to the specified submodule
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer PassOn<Event, Module>()
            where Event : class, IEvent
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by passing it on to the specified submodule
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being passed on</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer PassOn<EventIn, EventOut, Module>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeEvent<Event>()
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being exposed</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeEvent<EventIn, EventOut>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent;

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
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<Command, Module>()
            where Command : class, ICommand
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="CommandIn">The type of the command being handled</typeparam>
        /// <typeparam name="CommandOut">The type of the command being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <param name="mapping">A mapping between the commands</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<CommandIn, CommandOut, Module>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeCommand<Command>()
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="CommandIn">The type of the command being handled</typeparam>
        /// <typeparam name="CommandOut">The type of the command being exposed</typeparam>
        /// <param name="mapping">A mapping between the commands</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeCommand<CommandIn, CommandOut>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand;

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
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<Query, Response, Module>()
            where Query : class, IQuery<Response>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="QueryIn">The type of the query being handled</typeparam>
        /// <typeparam name="QueryOut">The type of the query being forwarded</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <param name="mapping">A mapping between the queries</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<QueryIn, QueryOut, Response, Module>(Func<QueryIn, QueryOut> mapping)
            where QueryIn : class, IQuery<Response>
            where QueryOut : class, IQuery<Response>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="QueryIn">The type of the query being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the query being handled</typeparam>
        /// <typeparam name="QueryOut">The type of the query being forwarded</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the query being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <param name="queryMapping">A mapping between the queries</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeQuery<Query, Response>()
            where Query : class, IQuery<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="QueryIn">The type of the query being handled</typeparam>
        /// <typeparam name="QueryOut">The type of the query being exposed</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <param name="mapping">A mapping between the queries</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeQuery<QueryIn, QueryOut, Response>(Func<QueryIn, QueryOut> mapping)
            where QueryIn : class, IQuery<Response>
            where QueryOut : class, IQuery<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="QueryIn">The type of the query being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the query being handled</typeparam>
        /// <typeparam name="QueryOut">The type of the query being exposed</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the query being exposed</typeparam>
        /// <param name="queryMapping">A mapping between the queries</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeQuery<QueryIn, ResponseIn, QueryOut, ResponseOut>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>;

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
