using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Inbox
{
    /// <summary>
    /// Lets you define incoming query messages that your module will handle.
    /// </summary>
    public interface IQueryInboxDefinition
    {
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

        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardsQuery<Query, Response, Module>()
            where Query : class, IQuery<Response>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Query">The type of the query being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the query</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the query</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardsQuery<Query, Response, Interceptor, Module>()
            where Query : class, IQuery<Response>
            where Interceptor : class, IQueryInterceptor<Query, Response>
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
        IInboxDefinition ForwardsQuery<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>query</b> message is handled by your module 
        /// by forwarding it to the specified submodule as another query
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
        IInboxDefinition ForwardsQuery<QueryIn, ResponseIn, QueryOut, ResponseOut, Interceptor, Module>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Interceptor : class, IQueryInterceptor<QueryOut, ResponseOut>
            where Module : TychoModule;
    }
}
