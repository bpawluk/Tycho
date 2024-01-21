namespace Tycho.Contract.Outbox
{
    /// <summary>
    /// Lets you define logic for handling messages sent out by a module that you want to use
    /// </summary>
    public interface IOutboxConsumer : IEventOutboxConsumer, ICommandOutboxConsumer, IQueryOutboxConsumer { }
}
