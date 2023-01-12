using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Contracts
{
    public interface IOutboxDefiner
    {
        IOutboxDefiner Publishes<Event>()
            where Event : class, IEvent;

        IOutboxDefiner Requires<Command>()
            where Command : class, ICommand;

        IOutboxDefiner Requires<Query, Response>()
            where Query : class, IQuery<Response>;
    }
}
