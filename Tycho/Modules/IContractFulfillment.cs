using System;
using Tycho.Requests;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for fulfilling the contract of a submodule
    /// </summary>
    public interface IContractFulfillment
    {
        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by exposing it as required by the module itself
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to expose</typeparam>
        IContractFulfillment Expose<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by exposing it as required by the module itself
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to expose</typeparam>
        /// <typeparam name="TResponse">The type of the request response</typeparam>
        IContractFulfillment Expose<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by exposing it as another request of type <typeparamref name="TTargetRequest"/> 
        /// required by the module itself
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to expose</typeparam>
        /// <typeparam name="TTargetRequest">The type of the target request exposed by the module</typeparam>
        /// <param name="map">A function to map the original request to the target request</param>
        IContractFulfillment ExposeAs<TRequest, TTargetRequest>(
            Func<TRequest, TTargetRequest> map)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by exposing it as another request of type <typeparamref name="TTargetRequest"/> 
        /// required by the module itself
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to expose</typeparam>
        /// <typeparam name="TResponse">The type of the original request response</typeparam>
        /// <typeparam name="TTargetRequest">The type of the target request exposed by the module</typeparam>
        /// <typeparam name="TTargetResponse">The type of the target request response</typeparam>
        /// <param name="mapRequest">A function to map the original request to the target request</param>
        /// <param name="mapResponse">A function to map the target request response to the original request response</param>
        IContractFulfillment ExposeAs<TRequest, TResponse, TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by forwarding it to <typeparamref name="TModule"/> module
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to forward</typeparam>
        /// <typeparam name="TModule">The type of the target module</typeparam>
        IContractFulfillment Forward<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by forwarding it to <typeparamref name="TModule"/> module
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to forward</typeparam>
        /// <typeparam name="TResponse">The type of the request response</typeparam>
        /// <typeparam name="TModule">The type of the target module</typeparam>
        IContractFulfillment Forward<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by forwarding it to <typeparamref name="TModule"/> module
        /// mapped as request of type <typeparamref name="TTargetRequest"/> 
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to forward</typeparam>
        /// <typeparam name="TTargetRequest">The type of the target request expected by the target module</typeparam>
        /// <typeparam name="TModule">The type of the target module</typeparam>
        /// <param name="map">A function to map the original request to the target request</param>
        IContractFulfillment ForwardAs<TRequest, TTargetRequest, TModule>(
            Func<TRequest, TTargetRequest> map)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by forwarding it to <typeparamref name="TModule"/> module
        /// mapped as request of type <typeparamref name="TTargetRequest"/> 
        /// </summary>
        /// <typeparam name="TRequest">The type of the original request to forward</typeparam>
        /// <typeparam name="TResponse">The type of the original request response</typeparam>
        /// <typeparam name="TTargetRequest">The type of the target request expected by the target module</typeparam>
        /// <typeparam name="TTargetResponse">The type of the target request response</typeparam>
        /// <typeparam name="TModule">The type of the target module</typeparam>
        /// <param name="mapRequest">A function to map the original request to the target request</param>
        /// <param name="mapResponse">A function to map the target request response to the original request response</param>
        IContractFulfillment ForwardAs<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TModule : TychoModule;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by handling it using <typeparamref name="THandler"/> handler
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to handle</typeparam>
        /// <typeparam name="THandler">The type of the handler to handle the request</typeparam>
        IContractFulfillment Handle<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by handling it using <typeparamref name="THandler"/> handler
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to handle</typeparam>
        /// <typeparam name="TResponse">The type of the request response</typeparam>
        /// <typeparam name="THandler">The type of the handler to handle the request</typeparam>
        IContractFulfillment Handle<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by ignoring it using a stub handler
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to ignore</typeparam>
        IContractFulfillment Ignore<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Fulfills a required request of type <typeparamref name="TRequest"/>
        /// by ignoring it using a stub handler that returns a default value of type <typeparamref name="TResponse"/>
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to ignore</typeparam>
        /// <typeparam name="TResponse">The type of the request response</typeparam>
        IContractFulfillment Ignore<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}