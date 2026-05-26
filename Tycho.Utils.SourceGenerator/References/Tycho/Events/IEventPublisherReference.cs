using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class IEventPublisherReference
    {
        private const string Namespace = "Tycho.Events.Publishing";
        private const string TypeName = "IEventPublisher";

        public static TypeModel TypeModel => new TypeModel(Namespace, TypeName);
    }
}
