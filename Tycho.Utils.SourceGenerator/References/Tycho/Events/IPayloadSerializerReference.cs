using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class IPayloadSerializerReference
    {
        private const string Namespace = "Tycho.Events.Serialization";
        private const string TypeName = "IPayloadSerializer";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
