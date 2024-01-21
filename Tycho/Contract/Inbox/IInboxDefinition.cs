namespace Tycho.Contract.Inbox
{
    /// <summary>
    /// Lets you define all the incoming messages that your module will handle.
    /// These messages make up the the interface that your module exposes to its clients
    /// </summary>
    public interface IInboxDefinition : IEventInboxDefinition, ICommandInboxDefinition, IQueryInboxDefinition { }
}
