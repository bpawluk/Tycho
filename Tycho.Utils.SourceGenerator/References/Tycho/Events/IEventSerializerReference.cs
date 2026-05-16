using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class IEventSerializerReference
    {
        private const string _namespace = "Tycho.Events.Serialization";
        private const string _typeName = "IEventSerializer";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}
