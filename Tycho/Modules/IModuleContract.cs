using Tycho.Requests;

namespace Tycho.Modules
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IModuleContract
    {
        /// <summary>
        ///     TODO
        /// </summary>
        IModuleContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IModuleContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IModuleContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        ///     TODO
        /// </summary>
        IModuleContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        ///     TODO
        /// </summary>
        IModuleContract Requires<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        ///     TODO
        /// </summary>
        IModuleContract Requires<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}