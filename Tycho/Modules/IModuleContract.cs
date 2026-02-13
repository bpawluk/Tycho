using System;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the contract of a Tycho module.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModuleContract
    {
        /// <summary>
        /// Declares that the module will forward all requests of type <typeparamref name="TRequest"/>
        /// to module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to forward.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the module will forward all requests of type <typeparamref name="TRequest"/>
        /// to module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to forward.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the module will forward all requests of type <typeparamref name="TRequest"/>
        /// to module <typeparamref name="TModule"/>, mapped as requests of type <typeparamref name="TTargetRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to forward.</typeparam>
        /// <typeparam name="TTargetRequest">The type of the request expected by the target module.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        /// <param name="mapRequest">Maps the original request to the target request.</param>
        /// <exception cref="ArgumentNullException"/>
        [ReferencedBySourceGenerator]
        IModuleContract ForwardsAs<TRequest, TTargetRequest, TModule>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the module will forward all requests of type <typeparamref name="TRequest"/>
        /// to module <typeparamref name="TModule"/>, mapped as requests of type <typeparamref name="TTargetRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to forward.</typeparam>
        /// <typeparam name="TResponse">The type of the original request response.</typeparam>
        /// <typeparam name="TTargetRequest">The type of the request expected by the target module.</typeparam>
        /// <typeparam name="TTargetResponse">The type of the target request response.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        /// <param name="mapRequest">Maps the original request to the target request.</param>
        /// <param name="mapResponse">Maps the target response to the original response.</param>
        /// <exception cref="ArgumentNullException"/>
        [ReferencedBySourceGenerator]
        IModuleContract ForwardsAs<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the module will handle all requests of type <typeparamref name="TRequest"/>
        /// using handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to handle.</typeparam>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Declares that the module will handle all requests of type <typeparamref name="TRequest"/>
        /// using handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to handle.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Declares that the module will execute requests of type <typeparamref name="TRequest"/>
        /// and requires them to be handled by its parent.
        /// </summary>
        /// <typeparam name="TRequest">The type of the required request.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Requires<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the module will execute requests of type <typeparamref name="TRequest"/>
        /// and requires them to be handled by its parent.
        /// </summary>
        /// <typeparam name="TRequest">The type of the required request.</typeparam>
        /// <typeparam name="TResponse">The type of the required request response.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Requires<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}
