using NewTycho.Events.Outbox;
using NewTycho.Requests.Outbox;

namespace NewTycho.Modules
{
    public interface IOutboxConsumer
    {
        IEventOutboxConsumer Events { get; }

        IRequestOutboxConsumer Requests { get; }
    }
}
