using System;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.Apps
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IContractFulfillment
    {
        /// <summary>
        ///     TODO
        /// </summary>
        IContractFulfillment Forward<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IContractFulfillment Forward<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IContractFulfillment ForwardAs<TRequest, TTargetRequest, TTargetModule>(
            Func<TRequest, TTargetRequest> map)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TTargetModule : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IContractFulfillment ForwardAs<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TTargetModule : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IContractFulfillment Handle<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        ///     TODO
        /// </summary>
        IContractFulfillment Handle<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        ///     TODO
        /// </summary>
        IContractFulfillment Ignore<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        ///     TODO
        /// </summary>
        IContractFulfillment Ignore<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}