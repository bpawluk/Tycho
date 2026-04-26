using System;
using System.Text.RegularExpressions;
using Tycho.Identity.Modules;
using Tycho.Modules;

namespace Tycho.Events.Routing.Steps
{
    internal class DownStreamRouteStep : IRouteStep
    {
        private const string _key = "DOWN";
        private const string _destinationGroup = "destination";

        private static readonly Regex _pattern = new Regex(@$"^{_key}\((?<{_destinationGroup}>.+)\)$", RegexOptions.IgnoreCase);

        public ModuleIdentity Destination { get; }

        private DownStreamRouteStep(ModuleIdentity destination)
        {
            Destination = destination;
        }

        public static DownStreamRouteStep Create<TModule>() where TModule : TychoModule
        {
            var moduleIdentity = ModuleIdentity.Create<TModule>();
            return new DownStreamRouteStep(moduleIdentity);
        }

        public override string ToString()
        {
            return $"{_key}({Destination})";
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

            var match = _pattern.Match(step);
            if (match.Success)
            {
                var destinationModuleIdentity = match.Groups[_destinationGroup].Value;
                var destinationModule = ModuleIdentity.Parse(destinationModuleIdentity);
                result = new DownStreamRouteStep(destinationModule);
                return true;
            }

            return false;
        }
    }
}