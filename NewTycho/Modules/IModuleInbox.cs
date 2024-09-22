using NewTycho.Events.Inbox;
using NewTycho.Requests.Inbox;

namespace NewTycho.Modules
{
    public interface IModuleInbox
    {
        IEventInboxDefinition Events { get; }

        IRequestInboxDefinition Requests { get; }
    }
}
