using System;

namespace Tycho.Events.Routing.Steps
{
    internal class FinalRouteStep : IRouteStep
    {
        private const string Key = "END";

        public static FinalRouteStep Create() => new FinalRouteStep();

        public override string ToString()
        {
            return Key;
        }

        public static IRouteStep Parse(string step)
        {
            if (TryParse(step, out IRouteStep? parsedStep))
            {
                return parsedStep;
            }
            throw new FormatException($"Invalid {nameof(IRouteStep)} format: {step}");
        }

        public static bool TryParse(string step, out IRouteStep result)
        {
            result = default!;

            if (Key.Equals(step, StringComparison.InvariantCultureIgnoreCase))
            {
                result = new FinalRouteStep();
                return true;
            }

            return false;
        }
    }
}
