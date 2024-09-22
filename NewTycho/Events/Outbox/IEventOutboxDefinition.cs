namespace NewTycho.Events.Outbox
{
    /// <summary>
    /// Lets you define all the outgoing <b>events</b> that your module will send out
    /// </summary>
    public interface IEventOutboxDefinition
    {
        /// <summary>
        /// Declares that the specified <b>event</b> is published by your module
        /// </summary>
        /// <typeparam name="TEvent">The type of the event being published</typeparam>
        IEventOutboxDefinition Declare<TEvent>()
            where TEvent : class, IEvent;
    }
}
