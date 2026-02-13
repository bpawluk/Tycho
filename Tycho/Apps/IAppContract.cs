using System;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for declaring the contract of a Tycho application.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IAppContract
    {
        /// <summary>
        /// Declares that the application will forward all requests of type <typeparamref name="TRequest"/>
        /// to module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to forward.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        [ReferencedBySourceGenerator]
        IAppContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the application will forward all requests of type <typeparamref name="TRequest"/>
        /// to module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to forward.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        [ReferencedBySourceGenerator]
        IAppContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the application will forward all requests of type <typeparamref name="TRequest"/>
        /// to module <typeparamref name="TModule"/>, mapped as requests of type <typeparamref name="TTargetRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to forward.</typeparam>
        /// <typeparam name="TTargetRequest">The type of the request expected by the target module.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        /// <param name="mapRequest">Maps the original request to the target request.</param>
        /// <exception cref="ArgumentNullException"/>
        [ReferencedBySourceGenerator]
        IAppContract ForwardsAs<TRequest, TTargetRequest, TModule>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the application will forward all requests of type <typeparamref name="TRequest"/>
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
        IAppContract ForwardsAs<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the application will handle all requests of type <typeparamref name="TRequest"/>
        /// using handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to handle.</typeparam>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        [ReferencedBySourceGenerator]
        IAppContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Declares that the application will handle all requests of type <typeparamref name="TRequest"/>
        /// using handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to handle.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        [ReferencedBySourceGenerator]
        IAppContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;
    }
}
