using System;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// Configures how an expected application request is handled or forwarded.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected request.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IAppRequestBinding<TRequest>
        where TRequest : class, IRequest
    {
        /// <summary>
        /// Declares that the application will handle the expected request using the handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        [ReferencedBySourceGenerator]
        IAppContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Forwards the expected request to a module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the request.</typeparam>
        IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the expected request to <typeparamref name="TTargetRequest"/> before forwarding.
        /// </summary>
        /// <typeparam name="TTargetRequest">The target request type.</typeparam>
        /// <param name="mapRequest">The request mapper.</param>
        IAppRequestBindingWithMapping<TRequest, TTargetRequest> MapsTo<TTargetRequest>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest;
    }

    /// <summary>
    /// Configures how an expected application request with a response is handled or forwarded.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected request.</typeparam>
    /// <typeparam name="TResponse">The type of the request response.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IAppRequestBinding<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        /// <summary>
        /// Declares that the application will handle the expected request using the handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        [ReferencedBySourceGenerator]
        IAppContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Forwards the expected request to a module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the request.</typeparam>
        IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the expected request and target response before forwarding.
        /// </summary>
        /// <typeparam name="TTargetRequest">The target request type.</typeparam>
        /// <typeparam name="TTargetResponse">The target request response type.</typeparam>
        /// <param name="mapRequest">The request mapper.</param>
        /// <param name="mapResponse">The response mapper.</param>
        IAppRequestBindingWithMapping<TRequest, TResponse, TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>;
    }
}
