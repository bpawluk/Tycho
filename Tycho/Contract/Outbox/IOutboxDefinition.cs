namespace Tycho.Contract.Outbox
{
    /// <summary>
    /// Lets you define all the outgoing messages that your module will send out.
    /// These messages make up the contract that your module requires its clients to fulfill
    /// </summary>
    public interface IOutboxDefinition
    {
        IEventOutboxDefinition Events { get; }

        IRequestOutboxDefinition Requests { get; }
    }
}
