using Tycho.Events;

namespace Tycho.Modules
{
    /// <summary>
    /// Configures routed delivery for a mapped module event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected event.</typeparam>
    /// <typeparam name="TTargetEvent">The target event type.</typeparam>
    public interface IModuleEventBindingWithMapping<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        /// <summary>
        /// Forwards the mapped event to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the mapped event.</typeparam>
        IModuleEventBindingWithMapping<TEvent, TTargetEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Exposes the mapped event to the module's parent.
        /// </summary>
        IModuleEventBindingWithMapping<TEvent, TTargetEvent> Exposes();
    }
}
