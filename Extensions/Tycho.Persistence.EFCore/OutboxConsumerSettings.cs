using System;

namespace Tycho.Persistence.EFCore;

public class OutboxConsumerSettings
{
    public int MaxDeliveryCount { get; set; } = 3;

    public TimeSpan ProcessingStateExpiration { get; set; } = TimeSpan.FromMinutes(5);
}