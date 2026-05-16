using System;

namespace Tycho.Events.Routing.Steps
{
    internal class UpStreamRouteStep : IRouteStep
    {
        private const string Key = "UP";

        public static UpStreamRouteStep Create() => new UpStreamRouteStep();

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
                result = new UpStreamRouteStep();
                return true;
            }

            return false;
        }
    }
}
