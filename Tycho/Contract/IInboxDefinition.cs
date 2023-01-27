using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Contract
{
    /// <summary>
    /// Lets you define all the incoming messages that your module will handle.
    /// These messages make up the the interface that your module exposes to its clients
    /// </summary>
    public interface IInboxDefinition
    {
        #region Events
        /// <summary>
        /// Declares that the specified <b>event</b> message is handled by your module 
        /// by passing it on to the specified submodule
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition PassesOn<Event, Module>()
            where Event : class, IEvent
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>event</b> message is handled by your module 
        /// by passing it on to the specified submodule as another event
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being passed on</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition PassesOn<EventIn, EventOut, Module>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>event</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="action">A method to be invoked when the event is published</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition SubscribesTo<Event>(Action<Event> action)
            where Event : class, IEvent;

        /// <summary>
        /// Declares that the specified <b>event</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="function">A method to be invoked when the event is published</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition SubscribesTo<Event>(Func<Event, Task> function)
            where Event : class, IEvent;

        /// <summary>
        /// Declares that the specified <b>event</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="function">A method to be invoked when the event is published</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition SubscribesTo<Event>(Func<Event, CancellationToken, Task> function)
            where Event : class, IEvent;

        /// <summary>
        /// Declares that the specified <b>event</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="handler">A handler to be used when the event is published</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition SubscribesTo<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent;

        /// <summary>
        /// Declares that the specified <b>event</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the event handler will be created each time the event is published
        /// </remarks>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the event is published</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition SubscribesTo<Event, Handler>()
            where Handler : class, IEventHandler<Event>
            where Event : class, IEvent;
        #endregion

        #region Commands
        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forwards<Command, Module>()
            where Command : class, ICommand
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// by forwarding it to the specified submodule as another command
        /// </summary>
        /// <typeparam name="CommandIn">The type of the command being handled</typeparam>
        /// <typeparam name="CommandOut">The type of the command being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <param name="mapping">A mapping between the commands</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forwards<CommandIn, CommandOut, Module>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="action">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Executes<Command>(Action<Command> action)
            where Command : class, ICommand;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="function">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Executes<Command>(Func<Command, Task> function)
            where Command : class, ICommand;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="function">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Executes<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="handler">A handler to be used when the command is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Executes<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the command handler will be created each time the command is received
        /// </remarks>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the command is received</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Executes<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand;
        #endregion

        #region Queries
        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forwards<Query, Response, Module>()
            where Query : class, IQuery<Response>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// by forwarding it to the specified submodule as another query
        /// </summary>
        /// <typeparam name="QueryIn">The type of the query being handled</typeparam>
        /// <typeparam name="QueryOut">The type of the query being forwarded</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <param name="mapping">A mapping between the queries</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forwards<QueryIn, QueryOut, Response, Module>(Func<QueryIn, QueryOut> mapping)
            where QueryIn : class, IQuery<Response>
            where QueryOut : class, IQuery<Response>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// by forwarding it to the specified submodule as another query
        /// </summary>
        /// <typeparam name="QueryIn">The type of the query being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the query being handled</typeparam>
        /// <typeparam name="QueryOut">The type of the query being forwarded</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the query being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <param name="queryMapping">A mapping between the queries</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forwards<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <param name="function">A method to be invoked when the query is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition RespondsTo<Query, Response>(Func<Query, Response> function)
            where Query : class, IQuery<Response>;

        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <param name="function">A method to be invoked when the query is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition RespondsTo<Query, Response>(Func<Query, Task<Response>> function)
            where Query : class, IQuery<Response>;

        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <param name="function">A method to be invoked when the query is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition RespondsTo<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
            where Query : class, IQuery<Response>;

        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <param name="handler">A handler to be used when the query is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition RespondsTo<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>;

        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the query handler will be created each time the query is received
        /// </remarks>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <typeparam name="Handler">A handler to be used when the query is received</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition RespondsTo<Query, Response, Handler>()
            where Handler : class, IQueryHandler<Query, Response>
            where Query : class, IQuery<Response>;
        #endregion
    }
}
