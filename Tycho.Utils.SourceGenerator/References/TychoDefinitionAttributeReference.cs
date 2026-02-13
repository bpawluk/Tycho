using Tycho.Utils.SourceGenerator.Model.Partial;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class TychoDefinitionAttributeReference
    {
        private const string _namespace = "Tycho";
        private const string _typeName = "TychoDefinitionAttribute";

        public static string TypeName => $"{_namespace}.{_typeName}";
        public static string GlobalTypeName => $"global::{TypeName}";
        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}
