using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging
{
    /// <summary>
    /// An interface for sending messages to the appropriate recipients
    /// </summary>
    public interface IMessageBroker
    {
        /// <summary>
        /// Pulishes an <b>event</b> that can be received by zero, one or multiple registered subscribers
        /// </summary>
        /// <typeparam name="Event">The type of the event being published</typeparam>
        /// <param name="eventData">An object that represents the event being published</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        void Publish<Event>(Event eventData, CancellationToken cancellationToken = default)
            where Event : class, IEvent;

        /// <summary>
        /// Executes a <b>request</b> that needs to be handled by exactly one registered recipient
        /// </summary>
        /// <typeparam name="Request">The type of the request being executed</typeparam>
        /// <param name="requestData">An object that represents the request being executed</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="KeyNotFoundException"/>
        Task Execute<Request>(Request requestData, CancellationToken cancellationToken = default)
            where Request : class, IRequest;

        /// <summary>
        /// Executes a <b>request</b> that needs to be handled by exactly one registered recipient
        /// which will return a response of the expected type
        /// </summary>
        /// <typeparam name="Request">The type of the request being executed</typeparam>
        /// <typeparam name="Response">The type of the request response</typeparam>
        /// <param name="requestData">An object that represents the request being executed</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation which returns the request response</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="KeyNotFoundException"/>
        Task<Response> Execute<Request, Response>(Request requestData, CancellationToken cancellationToken = default)
            where Request : class, IRequest<Response>;
    }
}
