using System.Collections.Generic;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging
{
    internal interface IMessageRouter
    {
        IEnumerable<IEventHandler<Event>> GetEventHandlers<Event>()
            where Event : class, IEvent;

        IRequestHandler<Request> GetRequestHandler<Request>()
            where Request : class, IRequest;

        IRequestHandler<Request, Response> GetRequestWithResponseHandler<Request, Response>()
            where Request : class, IRequest<Response>;

        void RegisterEventHandler<Event>(IEventHandler<Event> eventHandler)
            where Event : class, IEvent;

        void RegisterRequestHandler<Request>(IRequestHandler<Request> requestHandler)
            where Request : class, IRequest;

        void RegisterRequestWithResponseHandler<Request, Response>(IRequestHandler<Request, Response> requestHandler)
            where Request : class, IRequest<Response>;
    }
}
