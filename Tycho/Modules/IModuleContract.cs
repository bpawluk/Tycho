using System;
using Tycho.Requests;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the contract of a module
    /// </summary>
    public interface IModuleContract
    {
        /// <summary>
        /// Declares that the module will forward all requests 
        /// of type <typeparamref name="TRequest"/> to <typeparamref name="TModule"/> module
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to forward</typeparam>
        /// <typeparam name="TModule">The type of the target module</typeparam>
        IModuleContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the module will forward all requests 
        /// of type <typeparamref name="TRequest"/> to <typeparamref name="TModule"/> module
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to forward</typeparam>
        /// <typeparam name="TResponse">The type of the request response</typeparam>
        /// <typeparam name="TModule">The type of the target module</typeparam>
        IModuleContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the module will forward all requests 
        /// of type <typeparamref name="TRequest"/> to <typeparamref name="TModule"/> module
        /// mapped as requests of type <typeparamref name="TTargetRequest"/>
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to forward</typeparam>
        /// <typeparam name="TTargetRequest">The type of the target request expected by the target module</typeparam>
        /// <typeparam name="TModule">The type of the target module</typeparam>
        /// <param name="map">A function to map the original request to the target request</param>
        IModuleContract ForwardsAs<TRequest, TTargetRequest, TModule>(
            Func<TRequest, TTargetRequest> map)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the module will forward all requests 
        /// of type <typeparamref name="TRequest"/> to <typeparamref name="TModule"/> module
        /// mapped as requests of type <typeparamref name="TTargetRequest"/>
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to forward</typeparam>
        /// <typeparam name="TResponse">The type of the original request response</typeparam>
        /// <typeparam name="TTargetRequest">The type of the target request expected by the target module</typeparam>
        /// <typeparam name="TTargetResponse">The type of the target request response</typeparam>
        /// <typeparam name="TModule">The type of the target module</typeparam>
        /// <param name="mapRequest">A function to map the original request to the target request</param>
        /// <param name="mapResponse">A function to map the target request response to the original request response</param>
        IModuleContract ForwardsAs<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the module will handle all requests 
        /// of type <typeparamref name="TRequest"/> using <typeparamref name="THandler"/> handler
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to handle</typeparam>
        /// <typeparam name="THandler">The type of the handler to handle the request</typeparam>
        IModuleContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Declares that the module will handle all requests 
        /// of type <typeparamref name="TRequest"/> using <typeparamref name="THandler"/> handler
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to handle</typeparam>
        /// <typeparam name="TResponse">The type of the request response</typeparam>
        /// <typeparam name="THandler">The type of the handler to handle the request</typeparam>
        IModuleContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Declares that the module will execute requests of type <typeparamref name="TRequest"/>
        /// and requires them to be handled by its parent
        /// </summary>
        /// <typeparam name="TRequest">The type of the required request</typeparam>
        IModuleContract Requires<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the module will execute requests of type <typeparamref name="TRequest"/>
        /// and requires them to be handled by its parent
        /// </summary>
        /// <typeparam name="TRequest">The type of the required request</typeparam>
        /// <typeparam name="TResponse">The type of the required request response</typeparam>
        IModuleContract Requires<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}