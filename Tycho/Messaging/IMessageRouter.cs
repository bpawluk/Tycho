using System.Collections.Generic;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging
{
    internal interface IMessageRouter
    {
        IEnumerable<IEventHandler<Event>> GetEventHandlers<Event>()
            where Event : class, IEvent;

        ICommandHandler<Command> GetCommandHandler<Command>()
            where Command : class, ICommand;

        IQueryHandler<Query, Response> GetQueryHandler<Query, Response>()
            where Query : class, IQuery<Response>;

        void RegisterEventHandler<Event>(IEventHandler<Event> eventHandler)
            where Event : class, IEvent;

        void RegisterCommandHandler<Command>(ICommandHandler<Command> commandHandler)
            where Command : class, ICommand;

        void RegisterQueryHandler<Query, Response>(IQueryHandler<Query, Response> queryHandler)
            where Query : class, IQuery<Response>;
    }
}
