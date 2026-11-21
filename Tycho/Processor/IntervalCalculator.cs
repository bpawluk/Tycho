using System;

namespace Tycho.Processor
{
    internal sealed class IntervalCalculator : IIntervalCalculator
    {
        private readonly TimeSpan _initial;
        private readonly TimeSpan _maximal;
        private readonly double _multiplier;

        public TimeSpan Current { get; private set; }

        public IntervalCalculator(TimeSpan initial, TimeSpan maximal, double multiplier)
        {
            _initial = initial;
            _maximal = maximal;
            _multiplier = multiplier;

            Current = initial;
        }


        public void Increase()
        {
            double increasedTicks = Current.Ticks * _multiplier;

            if (increasedTicks >= _maximal.Ticks)
            {
                Current = _maximal;
                return;
            }

            long nextTicks = Math.Max(Current.Ticks + 1, (long)Math.Ceiling(increasedTicks));
            Current = TimeSpan.FromTicks(nextTicks);
        }

        public void Reset()
        {
            Current = _initial;
        }
    }
}
