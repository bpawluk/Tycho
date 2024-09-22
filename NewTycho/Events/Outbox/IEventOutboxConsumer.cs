using NewTycho.Modules;

namespace NewTycho.Events.Outbox
{
    /// <summary>
    /// Lets you define logic for handling <b>events</b> sent out by a module that you want to use
    /// </summary>
    public interface IEventOutboxConsumer
    {
        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// </summary>
        /// <typeparam name="TEvent">The type of the event being handled</typeparam>
        /// <typeparam name="THandler">A handler to be used when the event is published</typeparam>
        IEventOutboxConsumer Handle<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IHandle<TEvent>;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="TEvent">The type of the event being handled</typeparam>
        /// <typeparam name="TModule">The type of the submodule to receive the event</typeparam>
        IEventOutboxConsumer Forward<TEvent, TModule>()
            where TEvent : class, IEvent
            where TModule : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="TEvent">The type of the event being handled</typeparam>
        IEventOutboxConsumer Expose<TEvent>()
            where TEvent : class, IEvent;
    }
}
