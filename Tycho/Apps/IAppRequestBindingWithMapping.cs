using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.Apps
{
    /// <summary>
    /// Configures forwarding for a mapped application request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected request.</typeparam>
    /// <typeparam name="TTargetRequest">The target request type.</typeparam>
    public interface IAppRequestBindingWithMapping<TRequest, TTargetRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        /// <summary>
        /// Forwards the mapped request to a module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the mapped request.</typeparam>
        IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule;
    }

    /// <summary>
    /// Configures forwarding for a mapped application request with a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected request.</typeparam>
    /// <typeparam name="TResponse">The type of the request response.</typeparam>
    /// <typeparam name="TTargetRequest">The target request type.</typeparam>
    /// <typeparam name="TTargetResponse">The target request response type.</typeparam>
    public interface IAppRequestBindingWithMapping<TRequest, TResponse, TTargetRequest, TTargetResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        /// <summary>
        /// Forwards the mapped request to a module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the mapped request.</typeparam>
        IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule;
    }
}
