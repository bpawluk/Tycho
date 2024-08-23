using System;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox
{
    /// <summary>
    /// Lets you define logic for handling <b>events</b> sent out by a module that you want to use
    /// </summary>
    public interface IEventOutboxConsumer
    {
        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="handler">A handler to be used when the event is published</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the event handler will be created each time the event is published
        /// </remarks>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the event is published</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Handle<Event, Handler>()
            where Handler : class, IEventHandler<Event>
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<Event, Module>()
            where Event : class, IEvent
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being passed on</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Forward<EventIn, EventOut, Module>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the event interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardWithInterception<Event, Interceptor, Module>()
            where Event : class, IEvent
            where Interceptor : class, IEventInterceptor<Event>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being passed on</typeparam>
        /// <typeparam name="Interceptor">The type of the event interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardWithInterception<EventIn, EventOut, Interceptor, Module>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Interceptor : class, IEventInterceptor<EventOut>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Expose<Event>()
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being exposed</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer Expose<EventIn, EventOut>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the event interceptor being used</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeWithInterception<Event, Interceptor>()
            where Event : class, IEvent
            where Interceptor : class, IEventInterceptor<Event>;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b>
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being exposed</typeparam>
        /// <typeparam name="Interceptor">The type of the event interceptor being used</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeWithInterception<EventIn, EventOut, Interceptor>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Interceptor : class, IEventInterceptor<EventOut>;
    }
}
