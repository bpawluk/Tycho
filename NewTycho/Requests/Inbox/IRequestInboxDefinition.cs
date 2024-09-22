using NewTycho.Modules;
using NewTycho.Requests;

namespace NewTycho.Requests.Inbox
{
    /// <summary>
    /// Lets you define incoming <b>requests</b> that your module will handle
    /// </summary>
    public interface IRequestInboxDefinition
    {
        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being handled</typeparam>
        /// <typeparam name="THandler">A handler to be used when the request is received</typeparam>
        IRequestInboxDefinition Handle<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the request handler will be created each time the request is received
        /// </remarks>
        /// <typeparam name="TRequest">The type of the request being handled</typeparam>
        /// <typeparam name="TResponse">The type of the response expected by the request</typeparam>
        /// <typeparam name="THandler">A handler to be used when the request is received</typeparam>
        IRequestInboxDefinition Handle<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being handled</typeparam>
        /// <typeparam name="TModule">The type of the submodule to receive the request</typeparam>
        IRequestInboxDefinition Forward<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the specified <b>request</b> is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being handled</typeparam>
        /// <typeparam name="TResponse">The type of the response expected by the request</typeparam>
        /// <typeparam name="TModule">The type of the submodule to receive the request</typeparam>
        IRequestInboxDefinition Forward<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;
    }
}
