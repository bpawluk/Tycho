using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class EventSerializerBaseReference
    {
        private const string Namespace = "Tycho.Events.Serialization";
        private const string TypeName = "EventSerializerBase";

        public const string RegisterEventMethodName = "RegisterEvent";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
