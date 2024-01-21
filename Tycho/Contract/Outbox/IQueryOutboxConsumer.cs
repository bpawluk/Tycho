using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox
{
    /// <summary>
    /// Lets you define logic for handling query messages sent out by a module that you want to use
    /// </summary>
    public interface IQueryOutboxConsumer
    {
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
        /// Defines logic for handling the specified <b>query</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardQuery<Query, Response, Module>()
            where Query : class, IQuery<Response>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardQuery<Query, Response, Interceptor, Module>()
            where Query : class, IQuery<Response>
            where Interceptor : class, IQueryInterceptor<Query, Response>
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
        IOutboxConsumer ForwardQuery<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>query</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="QueryIn">The type of the query being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the query being handled</typeparam>
        /// <typeparam name="QueryOut">The type of the query being forwarded</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the query being forwarded</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <param name="queryMapping">A mapping between the queries</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardQuery<QueryIn, ResponseIn, QueryOut, ResponseOut, Interceptor, Module>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Interceptor : class, IQueryInterceptor<QueryOut, ResponseOut>
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
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeQuery<Query, Response, Interceptor>()
            where Query : class, IQuery<Response>
            where Interceptor : class, IQueryInterceptor<Query, Response>;

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
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="QueryIn">The type of the query being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the query being handled</typeparam>
        /// <typeparam name="QueryOut">The type of the query being exposed</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the query being exposed</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <param name="queryMapping">A mapping between the queries</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeQuery<QueryIn, ResponseIn, QueryOut, ResponseOut, Interceptor>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Interceptor : class, IQueryInterceptor<QueryOut, ResponseOut>;

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
    }
}
