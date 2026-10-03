using System;

namespace Tycho.Processor
{
    internal interface IIntervalCalculator
    {
        TimeSpan Current { get; }

        void Increase();

        void Reset();
    }
}
