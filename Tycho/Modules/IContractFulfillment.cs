using Tycho.Requests;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for fulfilling a submodule contract.
    /// </summary>
    public interface IContractFulfillment
    {
        /// <summary>
        /// Starts fulfillment of a required request of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to fulfill.</typeparam>
        IRequiredRequestBinding<TRequest> Fulfills<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Starts fulfillment of a required request of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to fulfill.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        IRequiredRequestBinding<TRequest, TResponse> Fulfills<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}
