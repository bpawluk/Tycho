using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging
{
    /// <summary>
    /// An interface for sending various kinds of messages to the appropriate recipients
    /// </summary>
    public interface IMessageBroker
    {
        /// <summary>
        /// Pulishes an <b>event</b> message than can be received by zero, one or multiple registered subscribers
        /// </summary>
        /// <typeparam name="Event">The type of the event being published</typeparam>
        /// <param name="eventData">An object that represents the event being published</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        void Publish<Event>(Event eventData, CancellationToken cancellationToken = default)
            where Event : class, IEvent;

        /// <summary>
        /// Sends a <b>command</b> message than needs to be handled by exactly one registered recipient
        /// </summary>
        /// <typeparam name="Command">The type of the command being executed</typeparam>
        /// <param name="commandData">An object that represents the command being executed</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="KeyNotFoundException"/>
        Task Execute<Command>(Command commandData, CancellationToken cancellationToken = default)
            where Command : class, ICommand;

        /// <summary>
        /// Sends a <b>query</b> message than needs to be handled by exactly one registered recipient 
        /// which will return a response of the expected type
        /// </summary>
        /// <typeparam name="Query">The type of the query being executed</typeparam>
        /// <typeparam name="Response">The type of the query response</typeparam>
        /// <param name="queryData">An object that represents the query being executed</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation which returns the query result</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="KeyNotFoundException"/>
        Task<Response> Execute<Query, Response>(Query queryData, CancellationToken cancellationToken = default)
            where Query : class, IQuery<Response>;
    }
}
