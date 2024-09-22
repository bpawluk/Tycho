using NewTycho.Modules;

namespace NewTycho.Requests
{
    /// <summary>
    /// Lets you define logic for handling <b>requests</b> sent out by a module that you want to use
    /// </summary>
    public interface IRequestOutboxConsumer
    {
        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being handled</typeparam>
        /// <typeparam name="THandler">A handler to be used when the request is received</typeparam>
        IRequestOutboxConsumer Handle<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the request handler will be created each time the request is received
        /// </remarks>
        /// <typeparam name="TRequest">The type of the request being handled</typeparam>
        /// <typeparam name="TResponse">The type of the request response</typeparam>
        /// <typeparam name="THandler">A handler to be used when the request is received</typeparam>
        IRequestOutboxConsumer Handle<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being handled</typeparam>
        /// <typeparam name="TModule">The type of the submodule to receive the request</typeparam>
        IRequestOutboxConsumer Forward<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being handled</typeparam>
        /// <typeparam name="TResponse">The type of the response expected by the request</typeparam>
        /// <typeparam name="TModule">The type of the submodule to receive the request</typeparam>
        IRequestOutboxConsumer Forward<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being handled</typeparam>
        IRequestOutboxConsumer Expose<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Defines logic for handling the specified <b>request</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being handled</typeparam>
        /// <typeparam name="TResponse">The type of the response expected by the request</typeparam>
        IRequestOutboxConsumer Expose<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;

        /// <summary>
        /// Ignores the specified <b>request</b> 
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being ignored</typeparam>
        IRequestOutboxConsumer Ignore<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Ignores the specified <b>request</b> and returns the provided default response
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being ignored</typeparam>
        /// <typeparam name="TResponse">The type of the request response</typeparam>
        /// <param name="response">The default response to return when the request is received</param>
        IRequestOutboxConsumer Ignore<TRequest, TResponse>(TResponse response)
            where TRequest : class, IRequest<TResponse>;
    }
}
