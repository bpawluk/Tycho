namespace Tycho.Contract.Outbox
{
    /// <summary>
    /// Lets you define logic for handling messages sent out by a module that you want to use
    /// </summary>
    public interface IOutboxConsumer
    {
        IEventOutboxConsumer Events { get; }

        IRequestOutboxConsumer Requests { get; }
    }
}
