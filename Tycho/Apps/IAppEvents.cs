using Tycho.Events;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for declaring the events expected by a Tycho application.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IAppEvents
    {
        /// <summary>
        /// Declares that the application expects events of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the expected event.</typeparam>
        [ReferencedBySourceGenerator]
        IAppEventBinding<TEvent> Expects<TEvent>()
            where TEvent : class, IEvent;
    }
}
