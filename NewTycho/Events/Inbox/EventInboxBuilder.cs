namespace NewTycho.Events.Inbox
{
    internal class EventInboxBuilder : IEventInboxDefinition
    {
        IEventInboxDefinition IEventInboxDefinition.Forward<TEvent, TModule>()
        {
            throw new System.NotImplementedException();
        }

        IEventInboxDefinition IEventInboxDefinition.Handle<TEvent, THandler>()
        {
            throw new System.NotImplementedException();
        }
    }
}
