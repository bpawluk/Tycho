using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class TychoDefinitionAttributeReference
    {
        private const string _namespace = "Tycho";
        private const string _typeName = "TychoDefinitionAttribute";

        public static string FullName => $"{_namespace}.{_typeName}";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}
