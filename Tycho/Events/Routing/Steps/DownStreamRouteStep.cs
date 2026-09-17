using System;
using System.Text.RegularExpressions;
using Tycho.Identity.Structure;
using Tycho.Modules;

namespace Tycho.Events.Routing.Steps
{
    internal class DownStreamRouteStep : IRouteStep
    {
        private const string Key = "DOWN";
        private const string DestinationGroup = "destination";

        private static readonly Regex s_pattern = new Regex(@$"^{Key}\((?<{DestinationGroup}>.+)\)$", RegexOptions.IgnoreCase);

        public DefinitionIdentity Destination { get; }

        private DownStreamRouteStep(DefinitionIdentity destination)
        {
            Destination = destination;
        }

        public static DownStreamRouteStep Create<TModule>() where TModule : TychoModule
        {
            var moduleIdentity = DefinitionIdentity.Create<TModule>();
            return new DownStreamRouteStep(moduleIdentity);
        }

        public override string ToString()
        {
            return $"{Key}({Destination})";
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

            Match match = s_pattern.Match(step);
            if (match.Success)
            {
                string destinationDefinitionIdentity = match.Groups[DestinationGroup].Value;
                var destinationModule = DefinitionIdentity.Parse(destinationDefinitionIdentity);
                result = new DownStreamRouteStep(destinationModule);
                return true;
            }

            return false;
        }
    }
}
