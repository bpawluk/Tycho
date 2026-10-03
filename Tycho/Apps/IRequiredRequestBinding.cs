using System;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.Apps
{
    /// <summary>
    /// A builder for fulfilling a required request without a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to fulfill.</typeparam>
    public interface IRequiredRequestBinding<TRequest>
        where TRequest : class, IRequest
    {
        /// <summary>
        /// Fulfills the required request by ignoring it using a stub Handler.
        /// </summary>
        IContractFulfillment Ignores();

        /// <summary>
        /// Fulfills the required request by handling it using the handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        IContractFulfillment HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Fulfills the required request by forwarding it to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the required request to another request type before it is fulfilled.
        /// </summary>
        /// <typeparam name="TTargetRequest">The type of the request expected by the target module.</typeparam>
        /// <param name="mapRequest">Maps the original request to the target request.</param>
        /// <exception cref="ArgumentNullException"/>
        IRequiredRequestBindingWithMapping<TTargetRequest> MapsTo<TTargetRequest>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest;
    }

    /// <summary>
    /// A builder for fulfilling a required request with a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to fulfill.</typeparam>
    /// <typeparam name="TResponse">The type of the request response.</typeparam>
    public interface IRequiredRequestBinding<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        /// <summary>
        /// Fulfills the required request by ignoring it using a stub Handler that returns a default response.
        /// </summary>
        IContractFulfillment Ignores();

        /// <summary>
        /// Fulfills the required request by handling it using the handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        IContractFulfillment HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Fulfills the required request by forwarding it to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the required request and target response before the request is fulfilled.
        /// </summary>
        /// <typeparam name="TTargetRequest">The type of the request expected by the target module.</typeparam>
        /// <typeparam name="TTargetResponse">The type of the target request response.</typeparam>
        /// <param name="mapRequest">Maps the original request to the target request.</param>
        /// <param name="mapResponse">Maps the target response to the original response.</param>
        /// <exception cref="ArgumentNullException"/>
        IRequiredRequestBindingWithMapping<TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>;
    }
}
