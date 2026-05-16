using System;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for fulfilling the contract of a module.
    /// </summary>
    public interface IContractFulfillment
    {
        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by forwarding it to module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to forward.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        IContractFulfillment Forward<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by forwarding it to module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to forward.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        IContractFulfillment Forward<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by forwarding it to module <typeparamref name="TModule"/>, mapped as a request of type
        /// <typeparamref name="TTargetRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to forward.</typeparam>
        /// <typeparam name="TTargetRequest">The type of the request expected by the target module.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        /// <param name="mapRequest">Maps the original request to the target request.</param>
        /// <exception cref="ArgumentNullException"/>
        IContractFulfillment ForwardAs<TRequest, TTargetRequest, TModule>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by forwarding it to module <typeparamref name="TModule"/>, mapped as a request of type
        /// <typeparamref name="TTargetRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to forward.</typeparam>
        /// <typeparam name="TResponse">The type of the original request response.</typeparam>
        /// <typeparam name="TTargetRequest">The type of the request expected by the target module.</typeparam>
        /// <typeparam name="TTargetResponse">The type of the target request response.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        /// <param name="mapRequest">Maps the original request to the target request.</param>
        /// <param name="mapResponse">Maps the target response to the original response.</param>
        /// <exception cref="ArgumentNullException"/>
        IContractFulfillment ForwardAs<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by handling it using handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to handle.</typeparam>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        IContractFulfillment Handle<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by handling it using handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to handle.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        IContractFulfillment Handle<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by ignoring it using a stub handler.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to ignore.</typeparam>
        IContractFulfillment Ignore<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by ignoring it using a stub handler that returns a default value of type <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to ignore.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        IContractFulfillment Ignore<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}
