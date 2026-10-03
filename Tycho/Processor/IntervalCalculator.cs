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
            if (initial <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(initial));
            }
            _initial = initial;

            if (maximal < initial)
            {
                throw new ArgumentOutOfRangeException(nameof(maximal));
            }
            _maximal = maximal;

            if (double.IsNaN(multiplier) || double.IsInfinity(multiplier) || multiplier <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier));
            }
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

            Current = TimeSpan.FromTicks((long)increasedTicks);
        }

        public void Reset()
        {
            Current = _initial;
        }
    }
}
