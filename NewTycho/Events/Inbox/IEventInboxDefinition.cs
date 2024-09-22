using NewTycho.Modules;

namespace NewTycho.Events.Inbox
{
    /// <summary>
    /// Lets you define incoming <b>events</b> that your module will handle
    /// </summary>
    public interface IEventInboxDefinition
    {
        /// <summary>
        /// Declares that the specified <b>event</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="TEvent">The type of the event being handled</typeparam>
        /// <typeparam name="THandler">A handler to be used when the event is published</typeparam>
        IEventInboxDefinition Handle<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IHandle<TEvent>;

        /// <summary>
        /// Declares that the specified <b>event</b> is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="TEvent">The type of the event being handled</typeparam>
        /// <typeparam name="TModule">The type of the submodule to receive the event</typeparam>
        IEventInboxDefinition Forward<TEvent, TModule>()
            where TEvent : class, IEvent
            where TModule : TychoModule;
    }
}
