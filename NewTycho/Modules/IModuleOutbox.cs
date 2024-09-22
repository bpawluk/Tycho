using NewTycho.Events.Outbox;
using NewTycho.Requests.Outbox;

namespace NewTycho.Modules
{
    public interface IModuleOutbox
    {
        IEventOutboxDefinition Events { get; }

        IRequestOutboxDefinition Requests { get; }
    }
}
