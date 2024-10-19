﻿using System;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.Apps
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IAppContract
    {
        /// <summary>
        ///     TODO
        /// </summary>
        IAppContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IAppContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IAppContract ForwardsAs<TRequest, TTargetRequest, TModule>(
            Func<TRequest, TTargetRequest> map)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IAppContract ForwardsAs<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TModule : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IAppContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        ///     TODO
        /// </summary>
        IAppContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;
    }
}