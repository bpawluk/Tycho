namespace Tycho.Messaging.Contracts
{
    public interface IInboxBuilder
    {
        IMessageBroker Build();
    }
}
