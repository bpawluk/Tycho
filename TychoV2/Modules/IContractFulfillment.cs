using TychoV2.Requests;

namespace TychoV2.Modules
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IContractFulfillment
    {
        /// <summary>
        /// TODO
        /// </summary>
        IModuleContract Expose<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// TODO
        /// </summary>
        IModuleContract Expose<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
        /// <summary>
        /// TODO
        /// </summary>
        IModuleContract Forward<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule;

        /// <summary>
        /// TODO
        /// </summary>
        IModuleContract Forward<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule;

        /// <summary>
        /// TODO
        /// </summary>
        IModuleContract Handle<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>;

        /// <summary>
        /// TODO
        /// </summary>
        IModuleContract Handle<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>;

        /// <summary>
        /// TODO
        /// </summary>
        IModuleContract Ignore<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// TODO
        /// </summary>
        IModuleContract Ignore<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}