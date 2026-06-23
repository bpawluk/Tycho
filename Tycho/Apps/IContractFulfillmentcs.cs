using System;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for fulfilling a module contract.
    /// </summary>
    public interface IContractFulfillment
    {
        /// <summary>
        /// Starts fulfillment of a required request of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to fulfill.</typeparam>
        IContractRequestFulfillment<TRequest> Fulfills<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Starts fulfillment of a required request of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to fulfill.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        IContractRequestFulfillment<TRequest, TResponse> Fulfills<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }

    /// <summary>
    /// A builder for fulfilling a required request without a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to fulfill.</typeparam>
    public interface IContractRequestFulfillment<TRequest>
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
        IMappedContractRequestFulfillment<TTargetRequest> MapsTo<TTargetRequest>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest;
    }

    /// <summary>
    /// A builder for fulfilling a required request with a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to fulfill.</typeparam>
    /// <typeparam name="TResponse">The type of the request response.</typeparam>
    public interface IContractRequestFulfillment<TRequest, TResponse>
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
        IMappedContractRequestFulfillment<TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>;
    }

    /// <summary>
    /// A builder for fulfilling a mapped request without a response.
    /// </summary>
    /// <typeparam name="TTargetRequest">The type of the mapped request.</typeparam>
    public interface IMappedContractRequestFulfillment<TTargetRequest>
        where TTargetRequest : class, IRequest
    {
        /// <summary>
        /// Fulfills the required request by forwarding the mapped request to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule;
    }

    /// <summary>
    /// A builder for fulfilling a mapped request with a response.
    /// </summary>
    /// <typeparam name="TTargetRequest">The type of the mapped request.</typeparam>
    /// <typeparam name="TTargetResponse">The type of the mapped request response.</typeparam>
    public interface IMappedContractRequestFulfillment<TTargetRequest, TTargetResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        /// <summary>
        /// Fulfills the required request by forwarding the mapped request to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule;
    }
}
