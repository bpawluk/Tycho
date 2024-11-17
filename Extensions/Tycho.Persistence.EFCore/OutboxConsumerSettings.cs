using System;

namespace Tycho.Persistence.EFCore;

/// <summary>
///     TODO
/// </summary>
public class OutboxConsumerSettings
{
    public uint MaxDeliveryCount { get; set; } = 3;

    public TimeSpan ProcessingStateExpiration { get; set; } = TimeSpan.FromMinutes(5);
}