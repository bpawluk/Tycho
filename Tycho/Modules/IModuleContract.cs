using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the contract of a Tycho module.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModuleContract
    {
        /// <summary>
        /// Declares that the module expects requests of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the expected request.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleRequestBinding<TRequest> Expects<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the module expects requests of type <typeparamref name="TRequest"/> with response <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the expected request.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleRequestBinding<TRequest, TResponse> Expects<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;

        /// <summary>
        /// Declares that the module will execute requests of type <typeparamref name="TRequest"/> and requires them to be handled by its parent.
        /// </summary>
        /// <typeparam name="TRequest">The type of the required request.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Requires<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the module will execute requests of type <typeparamref name="TRequest"/> and requires them to be handled by its parent.
        /// </summary>
        /// <typeparam name="TRequest">The type of the required request.</typeparam>
        /// <typeparam name="TResponse">The type of the required request response.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Requires<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}
