using Tycho.Events;
using Tycho.Utils;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the events expected by a module.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModuleEvents
    {
        /// <summary>
        /// Declares that the module expects events of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the expected event.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleEventBinding<TEvent> Expects<TEvent>()
            where TEvent : class, IEvent;
    }
}
