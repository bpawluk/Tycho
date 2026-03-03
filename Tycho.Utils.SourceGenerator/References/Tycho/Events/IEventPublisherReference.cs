using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class IEventPublisherReference
    {
        private const string _namespace = "Tycho.Events.Publishing";
        private const string _typeName = "IEventPublisher";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}
