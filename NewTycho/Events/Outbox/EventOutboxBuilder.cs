namespace NewTycho.Events.Outbox
{
    internal class EventOutboxBuilder : IEventOutboxConsumer, IEventOutboxDefinition
    {
        IEventOutboxDefinition IEventOutboxDefinition.Declare<TEvent>()
        {
            throw new System.NotImplementedException();
        }

        IEventOutboxConsumer IEventOutboxConsumer.Expose<TEvent>()
        {
            throw new System.NotImplementedException();
        }

        IEventOutboxConsumer IEventOutboxConsumer.Forward<TEvent, TModule>()
        {
            throw new System.NotImplementedException();
        }

        IEventOutboxConsumer IEventOutboxConsumer.Handle<TEvent, THandler>()
        {
            throw new System.NotImplementedException();
        }
    }
}
