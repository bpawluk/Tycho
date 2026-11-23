using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.Apps
{
    /// <summary>
    /// A builder for fulfilling a mapped request without a response.
    /// </summary>
    /// <typeparam name="TTargetRequest">The type of the mapped request.</typeparam>
    public interface IRequiredRequestBindingWithMapping<TTargetRequest>
        where TTargetRequest : class, IRequest
    {
        /// <summary>
        /// Fulfills the required request by forwarding the mapped request to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule;
    }

    /// <summary>
    /// A builder for fulfilling a mapped request with a response.
    /// </summary>
    /// <typeparam name="TTargetRequest">The type of the mapped request.</typeparam>
    /// <typeparam name="TTargetResponse">The type of the mapped request response.</typeparam>
    public interface IRequiredRequestBindingWithMapping<TTargetRequest, TTargetResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        /// <summary>
        /// Fulfills the required request by forwarding the mapped request to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule;
    }
}
