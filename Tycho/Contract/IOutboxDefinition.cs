using Tycho.Messaging.Payload;

namespace Tycho.Contract
{
    public interface IOutboxDefinition
    {
        IOutboxDefinition Publishes<Event>()
            where Event : class, IEvent;

        IOutboxDefinition Sends<Command>()
            where Command : class, ICommand;

        IOutboxDefinition Sends<Query, Response>()
            where Query : class, IQuery<Response>;
    }
}
