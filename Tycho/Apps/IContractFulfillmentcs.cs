using System;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for fulfilling the contract of a Module.
    /// </summary>
    public interface IContractFulfillment
    {
        /// <summary>
        /// Starts fulfillment of a required Request of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to fulfill.</typeparam>
        IContractRequestFulfillment<TRequest> Fulfills<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Starts fulfillment of a required Request of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to fulfill.</typeparam>
        /// <typeparam name="TResponse">The type of the Request response.</typeparam>
        IContractRequestFulfillment<TRequest, TResponse> Fulfills<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }

    /// <summary>
    /// A builder for fulfilling a required Request without a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the Request to fulfill.</typeparam>
    public interface IContractRequestFulfillment<TRequest>
        where TRequest : class, IRequest
    {
        /// <summary>
        /// Fulfills the required Request by ignoring it using a stub Handler.
        /// </summary>
        IContractFulfillment Ignores();

        /// <summary>
        /// Fulfills the required Request by handling it using Handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of Request Handler.</typeparam>
        IContractFulfillment HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Fulfills the required Request by forwarding it to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the required Request to another Request type before it is fulfilled.
        /// </summary>
        /// <typeparam name="TTargetRequest">The type of the Request expected by the target Module.</typeparam>
        /// <param name="mapRequest">Maps the original Request to the target Request.</param>
        /// <exception cref="ArgumentNullException"/>
        IMappedContractRequestFulfillment<TTargetRequest> MapsTo<TTargetRequest>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest;
    }

    /// <summary>
    /// A builder for fulfilling a required Request with a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the Request to fulfill.</typeparam>
    /// <typeparam name="TResponse">The type of the Request response.</typeparam>
    public interface IContractRequestFulfillment<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        /// <summary>
        /// Fulfills the required Request by ignoring it using a stub Handler that returns a default response.
        /// </summary>
        IContractFulfillment Ignores();

        /// <summary>
        /// Fulfills the required Request by handling it using Handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of Request Handler.</typeparam>
        IContractFulfillment HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Fulfills the required Request by forwarding it to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the required Request and target response before the Request is fulfilled.
        /// </summary>
        /// <typeparam name="TTargetRequest">The type of the Request expected by the target Module.</typeparam>
        /// <typeparam name="TTargetResponse">The type of the target Request response.</typeparam>
        /// <param name="mapRequest">Maps the original Request to the target Request.</param>
        /// <param name="mapResponse">Maps the target response to the original response.</param>
        /// <exception cref="ArgumentNullException"/>
        IMappedContractRequestFulfillment<TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>;
    }

    /// <summary>
    /// A builder for fulfilling a mapped Request without a response.
    /// </summary>
    /// <typeparam name="TTargetRequest">The type of the mapped Request.</typeparam>
    public interface IMappedContractRequestFulfillment<TTargetRequest>
        where TTargetRequest : class, IRequest
    {
        /// <summary>
        /// Fulfills the required Request by forwarding the mapped Request to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule;
    }

    /// <summary>
    /// A builder for fulfilling a mapped Request with a response.
    /// </summary>
    /// <typeparam name="TTargetRequest">The type of the mapped Request.</typeparam>
    /// <typeparam name="TTargetResponse">The type of the mapped Request response.</typeparam>
    public interface IMappedContractRequestFulfillment<TTargetRequest, TTargetResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        /// <summary>
        /// Fulfills the required Request by forwarding the mapped Request to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule;
    }
}
