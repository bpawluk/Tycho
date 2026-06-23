using System;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the contract of a Tycho Module.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModuleContract
    {
        /// <summary>
        /// Declares that the Module will forward all Requests of type <typeparamref name="TRequest"/>
        /// to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to forward.</typeparam>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the Module will forward all Requests of type <typeparamref name="TRequest"/>
        /// to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to forward.</typeparam>
        /// <typeparam name="TResponse">The type of the Request response.</typeparam>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the Module will forward all Requests of type <typeparamref name="TRequest"/>
        /// to Module <typeparamref name="TModule"/>, mapped as Requests of type <typeparamref name="TTargetRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the original Request to forward.</typeparam>
        /// <typeparam name="TTargetRequest">The type of the Request expected by the target Module.</typeparam>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        /// <param name="mapRequest">Maps the original Request to the target Request.</param>
        /// <exception cref="ArgumentNullException"/>
        [ReferencedBySourceGenerator]
        IModuleContract ForwardsAs<TRequest, TTargetRequest, TModule>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Declares that the Module will forward all Requests of type <typeparamref name="TRequest"/>
        /// to Module <typeparamref name="TModule"/>, mapped as Requests of type <typeparamref name="TTargetRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the original Request to forward.</typeparam>
        /// <typeparam name="TResponse">The type of the original Request response.</typeparam>
        /// <typeparam name="TTargetRequest">The type of the Request expected by the target Module.</typeparam>
        /// <typeparam name="TTargetResponse">The type of the target Request response.</typeparam>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        /// <param name="mapRequest">Maps the original Request to the target Request.</param>
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
        /// Declares that the Module will handle all Requests of type <typeparamref name="TRequest"/>
        /// using Handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to handle.</typeparam>
        /// <typeparam name="THandler">The type of Request Handler.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Declares that the Module will handle all Requests of type <typeparamref name="TRequest"/>
        /// using Handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to handle.</typeparam>
        /// <typeparam name="TResponse">The type of the Request response.</typeparam>
        /// <typeparam name="THandler">The type of Request Handler.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Declares that the Module will execute Requests of type <typeparamref name="TRequest"/>
        /// and requires them to be handled by its parent.
        /// </summary>
        /// <typeparam name="TRequest">The type of the required Request.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Requires<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the Module will execute Requests of type <typeparamref name="TRequest"/>
        /// and requires them to be handled by its parent.
        /// </summary>
        /// <typeparam name="TRequest">The type of the required Request.</typeparam>
        /// <typeparam name="TResponse">The type of the required Request response.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Requires<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}
