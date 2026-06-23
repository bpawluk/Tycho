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
        /// Declares that the Module expects Requests of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
        /// <returns>An expectation builder for the Request.</returns>
        [ReferencedBySourceGenerator]
        IModuleRequestExpectation<TRequest> Expects<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the Module expects Requests of type <typeparamref name="TRequest"/>
        /// with response <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
        /// <typeparam name="TResponse">The type of the Request response.</typeparam>
        /// <returns>An expectation builder for the Request.</returns>
        [ReferencedBySourceGenerator]
        IModuleRequestExpectation<TRequest, TResponse> Expects<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;

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

    /// <summary>
    /// Configures how an expected Module Request is handled or forwarded.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IModuleRequestExpectation<TRequest>
        where TRequest : class, IRequest
    {
        /// <summary>
        /// Declares that the Module will handle the expected Request using Handler
        /// <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of Request Handler.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Forwards the expected Request to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the Request.</typeparam>
        IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the expected Request to <typeparamref name="TTargetRequest"/> before forwarding.
        /// </summary>
        /// <typeparam name="TTargetRequest">The target Request type.</typeparam>
        /// <param name="mapRequest">The Request mapper.</param>
        IModuleMappedRequestExpectation<TRequest, TTargetRequest> MapsTo<TTargetRequest>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest;
    }

    /// <summary>
    /// Configures how an expected Module Request with a response is handled or forwarded.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
    /// <typeparam name="TResponse">The type of the Request response.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IModuleRequestExpectation<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        /// <summary>
        /// Declares that the Module will handle the expected Request using Handler
        /// <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of Request Handler.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Forwards the expected Request to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the Request.</typeparam>
        IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the expected Request and target response before forwarding.
        /// </summary>
        /// <typeparam name="TTargetRequest">The target Request type.</typeparam>
        /// <typeparam name="TTargetResponse">The target Request response type.</typeparam>
        /// <param name="mapRequest">The Request mapper.</param>
        /// <param name="mapResponse">The response mapper.</param>
        IModuleMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>;
    }

    /// <summary>
    /// Configures forwarding for a mapped Module Request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
    /// <typeparam name="TTargetRequest">The target Request type.</typeparam>
    public interface IModuleMappedRequestExpectation<TRequest, TTargetRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        /// <summary>
        /// Forwards the mapped Request to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the mapped Request.</typeparam>
        IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule;
    }

    /// <summary>
    /// Configures forwarding for a mapped Module Request with a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
    /// <typeparam name="TResponse">The type of the Request response.</typeparam>
    /// <typeparam name="TTargetRequest">The target Request type.</typeparam>
    /// <typeparam name="TTargetResponse">The target Request response type.</typeparam>
    public interface IModuleMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        /// <summary>
        /// Forwards the mapped Request to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the mapped Request.</typeparam>
        IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule;
    }
}
