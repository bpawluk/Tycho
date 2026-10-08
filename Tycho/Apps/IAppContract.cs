using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for declaring the requests expected by a Tycho application.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IAppContract
    {
        /// <summary>
        /// Declares that the application expects requests of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the expected request.</typeparam>
        [ReferencedBySourceGenerator]
        IAppRequestBinding<TRequest> Expects<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the application expects requests of type <typeparamref name="TRequest"/> with response <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the expected request.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        [ReferencedBySourceGenerator]
        IAppRequestBinding<TRequest, TResponse> Expects<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}
