using Tycho.Events;
using Tycho.Modules;

namespace Tycho.Apps
{
    /// <summary>
    /// Configures how an expected event is handled or routed within an application with mapping.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected event.</typeparam>
    /// <typeparam name="TTargetEvent">The target event type.</typeparam>
    public interface IAppEventBindingWithMapping<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        /// <summary>
        /// Forwards the mapped event to a module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the mapped event.</typeparam>
        IAppEventBindingWithMapping<TEvent, TTargetEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;
    }
}
