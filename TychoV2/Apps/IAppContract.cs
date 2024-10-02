﻿using TychoV2.Modules;
using TychoV2.Requests;

namespace TychoV2.Apps
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IAppContract
    {
        /// <summary>
        /// TODO
        /// </summary>
        IAppContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// TODO
        /// </summary>
        IAppContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        /// TODO
        /// </summary>
        IAppContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>;

        /// <summary>
        /// TODO
        /// </summary>
        IAppContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>;
    }
}