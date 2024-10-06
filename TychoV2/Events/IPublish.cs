﻿using System.Threading;
using System.Threading.Tasks;

namespace TychoV2.Events
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IPublish
    {
        Task Publish<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent;
    }
}
