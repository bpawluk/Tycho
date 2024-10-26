﻿using System;

namespace Tycho.Persistence
{
    public class OutboxProcessorSettings
    {
        public int ConcurrencyLimit { get; set; } = 10;

        public int BatchSize { get; set; } = 5;

        public TimeSpan InitialPollingInterval { get; set; } = TimeSpan.FromSeconds(0.2);

        public TimeSpan MaxPollingInterval { get; set; } = TimeSpan.FromSeconds(2.0);

        public double PollingIntervalMultiplier { get; set; } = 2.0;
    }
}