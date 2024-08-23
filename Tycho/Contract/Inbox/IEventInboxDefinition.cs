using System;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Inbox
{
    /// <summary>
    /// Lets you define incoming <b>events</b> that your module will handle.
    /// </summary>
    public interface IEventInboxDefinition
    {
        /// <summary>
        /// Declares that the specified <b>event</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="handler">A handler to be used when the event is published</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent;

        /// <summary>
        /// Declares that the specified <b>event</b> is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the event handler will be created each time the event is published
        /// </remarks>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the event is published</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Handle<Event, Handler>()
            where Handler : class, IEventHandler<Event>
            where Event : class, IEvent;

        /// <summary>
        /// Declares that the specified <b>event</b> is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forward<Event, Module>()
            where Event : class, IEvent
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>event</b> is handled by your module 
        /// by forwarding it to the specified submodule as another event
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Forward<EventIn, EventOut, Module>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>event</b> is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the event interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardWithInterception<Event, Interceptor, Module>()
            where Event : class, IEvent
            where Interceptor : class, IEventInterceptor<Event>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>event</b> is handled by your module 
        /// by forwarding it to the specified submodule as another event
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being forwarded</typeparam>
        /// <typeparam name="Interceptor">The type of the event interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardWithInterception<EventIn, EventOut, Interceptor, Module>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Interceptor : class, IEventInterceptor<EventOut>
            where Module : TychoModule;
    }
}
