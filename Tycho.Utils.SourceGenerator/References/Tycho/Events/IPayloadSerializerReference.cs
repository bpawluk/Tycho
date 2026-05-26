using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class IPayloadSerializerReference
    {
        private const string Namespace = "Tycho.Events.Serialization";
        private const string TypeName = "IPayloadSerializer";

        public static TypeModel TypeModel => new TypeModel(Namespace, TypeName);
    }
}
