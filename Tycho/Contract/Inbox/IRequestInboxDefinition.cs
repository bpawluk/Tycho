using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Inbox
{
    /// <summary>
    /// Lets you define incoming <b>requests</b> that your module will handle.
    /// </summary>
    public interface IRequestInboxDefinition
    {
        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <param name="action">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Request>(Action<Request> action)
            where Request : class, IRequest;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <param name="function">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Request, Response>(Func<Request, Response> function)
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <param name="function">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Request>(Func<Request, Task> function)
            where Request : class, IRequest;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <param name="function">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Request, Response>(Func<Request, Task<Response>> function)
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <param name="function">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Request>(Func<Request, CancellationToken, Task> function)
            where Request : class, IRequest;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <param name="function">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Request, Response>(Func<Request, CancellationToken, Task<Response>> function)
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <param name="handler">A handler to be used when the request is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Request>(IRequestHandler<Request> handler)
            where Request : class, IRequest;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <param name="handler">A handler to be used when the request is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Request, Response>(IRequestHandler<Request, Response> handler)
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the request handler will be created each time the request is received
        /// </remarks>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the request is received</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Request, Handler>()
            where Handler : class, IRequestHandler<Request>
            where Request : class, IRequest;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the request handler will be created each time the request is received
        /// </remarks>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <typeparam name="Handler">A handler to be used when the request is received</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Request, Response, Handler>()
            where Handler : class, IRequestHandler<Request, Response>
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forward<Request, Module>()
            where Request : class, IRequest
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forward<Request, Response, Module>()
            where Request : class, IRequest<Response>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// by forwarding it to the specified submodule as another Request
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <param name="mapping">A mapping between the requests</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forward<RequestIn, RequestOut, Module>(Func<RequestIn, RequestOut> mapping)
            where RequestIn : class, IRequest
            where RequestOut : class, IRequest
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// by forwarding it to the specified submodule as another Request
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being forwarded</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the request being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <param name="requestMapping">A mapping between the requests</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forward<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>(
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where RequestIn : class, IRequest<ResponseIn>
            where RequestOut : class, IRequest<ResponseOut>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardWithInterception<Request, Interceptor, Module>()
            where Request : class, IRequest
            where Interceptor : class, IRequestInterceptor<Request>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardWithInterception<Request, Response, Interceptor, Module>()
            where Request : class, IRequest<Response>
            where Interceptor : class, IRequestInterceptor<Request, Response>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// by forwarding it to the specified submodule as another Request
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being forwarded</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <param name="mapping">A mapping between the requests</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardWithInterception<RequestIn, RequestOut, Interceptor, Module>(Func<RequestIn, RequestOut> mapping)
            where RequestIn : class, IRequest
            where RequestOut : class, IRequest
            where Interceptor : class, IRequestInterceptor<RequestOut>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// by forwarding it to the specified submodule as another Request
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being forwarded</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the request being forwarded</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <param name="requestMapping">A mapping between the requests</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardWithInterception<RequestIn, ResponseIn, RequestOut, ResponseOut, Interceptor, Module>(
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where RequestIn : class, IRequest<ResponseIn>
            where RequestOut : class, IRequest<ResponseOut>
            where Interceptor : class, IRequestInterceptor<RequestOut, ResponseOut>
            where Module : TychoModule;
    }
}
