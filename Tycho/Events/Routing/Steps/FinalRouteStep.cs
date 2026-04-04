using System;

namespace Tycho.Events.Routing.Steps
{
    internal class FinalRouteStep : IRouteStep
    {
        private const string _key = "END";

        public static FinalRouteStep Create() => new FinalRouteStep();

        public override string ToString()
        {
            return _key;
        }

        public static IRouteStep Parse(string step)
        {
            if (TryParse(step, out var parsedStep))
            {
                return parsedStep;
            }
            throw new FormatException($"Invalid {nameof(IRouteStep)} format: {step}");
        }

        public static bool TryParse(string step, out IRouteStep result)
        {
            result = default!;

            if (_key.Equals(step, StringComparison.InvariantCultureIgnoreCase))
            {
                result = new FinalRouteStep();
                return true;
            }

            return false;
        }
    }
}
