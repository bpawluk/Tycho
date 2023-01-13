using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Contracts
{
    public interface IOutboxProducer
    {
        IOutboxProducer Publishes<Event>()
            where Event : class, IEvent;

        IOutboxProducer Sends<Command>()
            where Command : class, ICommand;

        IOutboxProducer Sends<Query, Response>()
            where Query : class, IQuery<Response>;
    }
}
