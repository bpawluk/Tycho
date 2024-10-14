﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IEventHandler 
    {
        Type EventType { get; }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IEventHandler<TEvent> : IEventHandler
        where TEvent : class, IEvent 
    {
        Type IEventHandler.EventType => typeof(TEvent);

        Task Handle(TEvent eventData, CancellationToken cancellationToken = default);
    }
}