namespace Tycho.Messaging.Contracts
{
    public interface IOutboxBuilder
    {
        IMessageBroker Build();
    }
}
