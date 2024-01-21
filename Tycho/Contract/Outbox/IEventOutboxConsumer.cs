﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox
{
    /// <summary>
    /// Lets you define logic for handling event messages sent out by a module that you want to use
    /// </summary>
    public interface IEventOutboxConsumer
    {
        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="action">A method to be invoked when the event is published</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleEvent<Event>(Action<Event> action)
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="function">A method to be invoked when the event is published</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleEvent<Event>(Func<Event, Task> function)
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="function">A method to be invoked when the event is published</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleEvent<Event>(Func<Event, CancellationToken, Task> function)
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <param name="handler">A handler to be used when the event is published</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleEvent<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the event handler will be created each time the event is published
        /// </remarks>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the event is published</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleEvent<Event, Handler>()
            where Handler : class, IEventHandler<Event>
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardEvent<Event, Module>()
            where Event : class, IEvent
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardEvent<Event, Interceptor, Module>()
            where Event : class, IEvent
            where Interceptor : class, IEventInterceptor<Event>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being passed on</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardEvent<EventIn, EventOut, Module>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being passed on</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the event</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardEvent<EventIn, EventOut, Interceptor, Module>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Interceptor : class, IEventInterceptor<EventOut>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeEvent<Event>()
            where Event : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Event">The type of the event being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeEvent<Event, Interceptor>()
            where Event : class, IEvent
            where Interceptor : class, IEventInterceptor<Event>;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being exposed</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeEvent<EventIn, EventOut>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent;

        /// <summary>
        /// Defines logic for handling the specified <b>event</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="EventIn">The type of the event being handled</typeparam>
        /// <typeparam name="EventOut">The type of the event being exposed</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <param name="mapping">A mapping between the events</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeEvent<EventIn, EventOut, Interceptor>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Interceptor : class, IEventInterceptor<EventOut>;
    }
}
