using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho
{
    internal static class TychoDefinitionAttributeReference
    {
        private const string Namespace = "Tycho";
        private const string TypeName = "TychoDefinitionAttribute";

        public static string FullName => $"{Namespace}.{TypeName}";

        public static TypeModel TypeModel => new TypeModel(Namespace, ImmutableEquatableArray<string>.Empty, TypeName);
    }
}
