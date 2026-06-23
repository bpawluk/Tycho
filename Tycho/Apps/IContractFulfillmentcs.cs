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
        /// Fulfills a required Request of type <typeparamref name="TRequest"/>
        /// by forwarding it to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to forward.</typeparam>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        IContractFulfillment Forward<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required Request of type <typeparamref name="TRequest"/>
        /// by forwarding it to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to forward.</typeparam>
        /// <typeparam name="TResponse">The type of the Request response.</typeparam>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        IContractFulfillment Forward<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required Request of type <typeparamref name="TRequest"/>
        /// by forwarding it to Module <typeparamref name="TModule"/>, mapped as a Request of type
        /// <typeparamref name="TTargetRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the original Request to forward.</typeparam>
        /// <typeparam name="TTargetRequest">The type of the Request expected by the target Module.</typeparam>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        /// <param name="mapRequest">Maps the original Request to the target Request.</param>
        /// <exception cref="ArgumentNullException"/>
        IContractFulfillment ForwardAs<TRequest, TTargetRequest, TModule>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required Request of type <typeparamref name="TRequest"/>
        /// by forwarding it to Module <typeparamref name="TModule"/>, mapped as a Request of type
        /// <typeparamref name="TTargetRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the original Request to forward.</typeparam>
        /// <typeparam name="TResponse">The type of the original Request response.</typeparam>
        /// <typeparam name="TTargetRequest">The type of the Request expected by the target Module.</typeparam>
        /// <typeparam name="TTargetResponse">The type of the target Request response.</typeparam>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        /// <param name="mapRequest">Maps the original Request to the target Request.</param>
        /// <param name="mapResponse">Maps the target response to the original response.</param>
        /// <exception cref="ArgumentNullException"/>
        IContractFulfillment ForwardAs<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required Request of type <typeparamref name="TRequest"/>
        /// by handling it using Handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to handle.</typeparam>
        /// <typeparam name="THandler">The type of Request Handler.</typeparam>
        IContractFulfillment Handle<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Fulfills a required Request of type <typeparamref name="TRequest"/>
        /// by handling it using Handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to handle.</typeparam>
        /// <typeparam name="TResponse">The type of the Request response.</typeparam>
        /// <typeparam name="THandler">The type of Request Handler.</typeparam>
        IContractFulfillment Handle<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Fulfills a required Request of type <typeparamref name="TRequest"/>
        /// by ignoring it using a stub Handler.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to ignore.</typeparam>
        IContractFulfillment Ignore<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Fulfills a required Request of type <typeparamref name="TRequest"/>
        /// by ignoring it using a stub Handler that returns a default value of type <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the Request to ignore.</typeparam>
        /// <typeparam name="TResponse">The type of the Request response.</typeparam>
        IContractFulfillment Ignore<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}
