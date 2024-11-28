using Tycho.Events;
using Tycho.Modules.Routing;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the events expected by a module
    /// </summary>
    public interface IModuleEvents
    {
        /// <summary>
        /// Declares that the module will handle all events 
        /// of type <typeparamref name="TEvent"/> using <typeparamref name="THandler"/> handler
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to handle</typeparam>
        /// <typeparam name="THandler">The type of the handler to handle the event</typeparam>
        IModuleEvents Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>;

        /// <summary>
        /// Declares that the module will route all events 
        /// of type <typeparamref name="TEvent"/> according to the configuration 
        /// defined using the returned instance of <see cref="IEventRouting{TEvent}"/>
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to route</typeparam>
        /// <returns>An instance of <see cref="IEventRouting{TEvent}"/> to configure event routing</returns>
        IEventRouting<TEvent> Routes<TEvent>()
            where TEvent : class, IEvent;
    }
}