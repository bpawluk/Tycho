using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox
{
    /// <summary>
    /// Lets you define logic for handling <b>requests</b> sent out by a module that you want to use
    /// </summary>
    public interface IRequestOutboxConsumer
    {
        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <param name="action">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Request>(Action<Request> action)
            where Request : class, IRequest;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the request response</typeparam>
        /// <param name="function">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Request, Response>(Func<Request, Response> function)
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <param name="function">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Request>(Func<Request, Task> function)
            where Request : class, IRequest;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the request response</typeparam>
        /// <param name="function">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Request, Response>(Func<Request, Task<Response>> function)
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <param name="function">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Request>(Func<Request, CancellationToken, Task> function)
            where Request : class, IRequest;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the request response</typeparam>
        /// <param name="function">A method to be invoked when the request is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Request, Response>(Func<Request, CancellationToken, Task<Response>> function)
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <param name="handler">A handler to be used when the request is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Request>(IRequestHandler<Request> handler)
            where Request : class, IRequest;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the request response</typeparam>
        /// <param name="handler">A handler to be used when the request is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Request, Response>(IRequestHandler<Request, Response> handler)
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the request handler will be created each time the request is received
        /// </remarks>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the request is received</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Request, Handler>()
            where Handler : class, IRequestHandler<Request>
            where Request : class, IRequest;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the request handler will be created each time the request is received
        /// </remarks>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the request response</typeparam>
        /// <typeparam name="Handler">A handler to be used when the request is received</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Request, Response, Handler>()
            where Handler : class, IRequestHandler<Request, Response>
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<Request, Module>()
            where Request : class, IRequest
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<Request, Response, Module>()
            where Request : class, IRequest<Response>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <param name="mapping">A mapping between the requests</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<RequestIn, RequestOut, Module>(Func<RequestIn, RequestOut> mapping)
            where RequestIn : class, IRequest
            where RequestOut : class, IRequest
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being forwarded</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the request being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <param name="RequestMapping">A mapping between the requests</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>(
            Func<RequestIn, RequestOut> RequestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where RequestIn : class, IRequest<ResponseIn>
            where RequestOut : class, IRequest<ResponseOut>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardWithInterception<Request, Interceptor, Module>()
            where Request : class, IRequest
            where Interceptor : class, IRequestInterceptor<Request>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardWithInterception<Request, Response, Interceptor, Module>()
            where Request : class, IRequest<Response>
            where Interceptor : class, IRequestInterceptor<Request, Response>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being forwarded</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <param name="mapping">A mapping between the requests</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardWithInterception<RequestIn, RequestOut, Interceptor, Module>(Func<RequestIn, RequestOut> mapping)
            where RequestIn : class, IRequest
            where RequestOut : class, IRequest
            where Interceptor : class, IRequestInterceptor<RequestOut>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being forwarded</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the request being forwarded</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the request</typeparam>
        /// <param name="RequestMapping">A mapping between the requests</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardWithInterception<RequestIn, ResponseIn, RequestOut, ResponseOut, Interceptor, Module>(
            Func<RequestIn, RequestOut> RequestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where RequestIn : class, IRequest<ResponseIn>
            where RequestOut : class, IRequest<ResponseOut>
            where Interceptor : class, IRequestInterceptor<RequestOut, ResponseOut>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Expose<Request>()
            where Request : class, IRequest;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Expose<Request, Response>()
            where Request : class, IRequest<Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being exposed</typeparam>
        /// <param name="mapping">A mapping between the requests</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Expose<RequestIn, RequestOut>(Func<RequestIn, RequestOut> mapping)
            where RequestIn : class, IRequest
            where RequestOut : class, IRequest;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being exposed</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the request being exposed</typeparam>
        /// <param name="RequestMapping">A mapping between the requests</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Expose<RequestIn, ResponseIn, RequestOut, ResponseOut>(
            Func<RequestIn, RequestOut> RequestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where RequestIn : class, IRequest<ResponseIn>
            where RequestOut : class, IRequest<ResponseOut>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeWithInterception<Request, Interceptor>()
            where Request : class, IRequest
            where Interceptor : class, IRequestInterceptor<Request>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Request">The type of the request being handled</typeparam>
        /// <typeparam name="Response">The type of the response expected by the request</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeWithInterception<Request, Response, Interceptor>()
            where Request : class, IRequest<Response>
            where Interceptor : class, IRequestInterceptor<Request, Response>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being exposed</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <param name="mapping">A mapping between the requests</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeWithInterception<RequestIn, RequestOut, Interceptor>(Func<RequestIn, RequestOut> mapping)
            where RequestIn : class, IRequest
            where RequestOut : class, IRequest
            where Interceptor : class, IRequestInterceptor<RequestOut>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="RequestIn">The type of the request being handled</typeparam>
        /// <typeparam name="ResponseIn">The type of the response expected by the request being handled</typeparam>
        /// <typeparam name="RequestOut">The type of the request being exposed</typeparam>
        /// <typeparam name="ResponseOut">The type of the response expected by the request being exposed</typeparam>
        /// <typeparam name="Interceptor">The type of the request interceptor being used</typeparam>
        /// <param name="RequestMapping">A mapping between the requests</param>
        /// <param name="responseMapping">A mapping between the responses</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeWithInterception<RequestIn, ResponseIn, RequestOut, ResponseOut, Interceptor>(
            Func<RequestIn, RequestOut> RequestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where RequestIn : class, IRequest<ResponseIn>
            where RequestOut : class, IRequest<ResponseOut>
            where Interceptor : class, IRequestInterceptor<RequestOut, ResponseOut>;

        /// <summary>
        /// Ignores the specified <b>request</b> 
        /// </summary>
        /// <typeparam name="Request">The type of the request being ignored</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Ignore<Request>()
            where Request : class, IRequest;

        /// <summary>
        /// Ignores the specified <b>request</b> and returns the provided default response
        /// </summary>
        /// <typeparam name="Request">The type of the request being ignored</typeparam>
        /// <typeparam name="Response">The type of the request response</typeparam>
        /// <param name="response">The default response to return when the request is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Ignore<Request, Response>(Response response)
            where Request : class, IRequest<Response>;
    }
}
