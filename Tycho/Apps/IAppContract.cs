using System;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for declaring the Requests expected by a Tycho Application.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IAppContract
    {
        /// <summary>
        /// Declares that the Application expects Requests of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
        /// <returns>An expectation builder for the Request.</returns>
        [ReferencedBySourceGenerator]
        IAppRequestExpectation<TRequest> Expects<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the Application expects Requests of type <typeparamref name="TRequest"/>
        /// with response <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
        /// <typeparam name="TResponse">The type of the Request response.</typeparam>
        /// <returns>An expectation builder for the Request.</returns>
        [ReferencedBySourceGenerator]
        IAppRequestExpectation<TRequest, TResponse> Expects<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }

    /// <summary>
    /// Configures how an expected Application Request is handled or forwarded.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IAppRequestExpectation<TRequest>
        where TRequest : class, IRequest
    {
        /// <summary>
        /// Declares that the Application will handle the expected Request using Handler
        /// <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of Request Handler.</typeparam>
        [ReferencedBySourceGenerator]
        IAppContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Forwards the expected Request to a Module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the Request.</typeparam>
        IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the expected Request to <typeparamref name="TTargetRequest"/> before forwarding.
        /// </summary>
        /// <typeparam name="TTargetRequest">The target Request type.</typeparam>
        /// <param name="mapRequest">The Request mapper.</param>
        IAppMappedRequestExpectation<TRequest, TTargetRequest> MapsTo<TTargetRequest>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest;
    }

    /// <summary>
    /// Configures how an expected Application Request with a response is handled or forwarded.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
    /// <typeparam name="TResponse">The type of the Request response.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IAppRequestExpectation<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        /// <summary>
        /// Declares that the Application will handle the expected Request using Handler
        /// <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of Request Handler.</typeparam>
        [ReferencedBySourceGenerator]
        IAppContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Forwards the expected Request to a Module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the Request.</typeparam>
        IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the expected Request and target response before forwarding.
        /// </summary>
        /// <typeparam name="TTargetRequest">The target Request type.</typeparam>
        /// <typeparam name="TTargetResponse">The target Request response type.</typeparam>
        /// <param name="mapRequest">The Request mapper.</param>
        /// <param name="mapResponse">The response mapper.</param>
        IAppMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>;
    }

    /// <summary>
    /// Configures forwarding for a mapped Application Request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
    /// <typeparam name="TTargetRequest">The target Request type.</typeparam>
    public interface IAppMappedRequestExpectation<TRequest, TTargetRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        /// <summary>
        /// Forwards the mapped Request to a Module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the mapped Request.</typeparam>
        IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule;
    }

    /// <summary>
    /// Configures forwarding for a mapped Application Request with a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected Request.</typeparam>
    /// <typeparam name="TResponse">The type of the Request response.</typeparam>
    /// <typeparam name="TTargetRequest">The target Request type.</typeparam>
    /// <typeparam name="TTargetResponse">The target Request response type.</typeparam>
    public interface IAppMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        /// <summary>
        /// Forwards the mapped Request to a Module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the mapped Request.</typeparam>
        IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule;
    }
}
